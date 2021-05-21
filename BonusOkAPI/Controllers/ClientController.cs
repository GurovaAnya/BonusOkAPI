using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BonusOkAPI.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BonusOkAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BonusOkAPI.Controllers
{
    /// <summary>
    /// Работа с картами клиентов
    /// </summary>
    /// <response code="403"> Токен неправильный </response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]

    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly BonusOkContext _context;
        private readonly IMapper _mapper;

        public ClientController(BonusOkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ClientConroller
        /// <summary>
        /// Создано для тестовых целей, не использовать в приложении
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientResponse>>> GetClients()
        {
            var data = await _context.Clients.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<Client>, IEnumerable<ClientResponse>>(data));
        }

        // GET: api/ClientConroller/5
        //[Authorize(Roles = Models.Client.Role)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponse>> GetClient([FromHeader(Name = "Authorization")]string JWT, int id)
        {
            var client = RightCredentials(id).Result;

            if (client == null)
            {
                return BadRequest();
            }

            return Ok(_mapper.Map<Client, ClientResponse>(client));
        }
        
        // GET: api/ClientConroller/5/Promo
        //[Authorize(Roles = Models.Client.Role)]
        [HttpGet("{id}/Promo")]
        public async Task<ActionResult<IEnumerable<PromoResponse>>> GetClientsPromos([FromHeader(Name = "Authorization")]string JWT,int id)
        {
            if (RightCredentials(id).Result == null)
            {
                return BadRequest();
            }
            
            var data = await _context.Clients
                .Where(c => c.Id == id)
                .Include(c => c.Promos)
                .SelectMany(c => c.Promos)
                .Where(c => c.EndDate >= DateTime.Now)
                .ToArrayAsync();
            return Ok(_mapper.Map<IEnumerable<Promo>, IEnumerable<PromoResponse>>(data));
        }
        
        // GET: api/ClientConroller/5/Promo/1
        //[Authorize(Roles = Models.Client.Role)]
        [HttpGet("{id}/Promo/{promoId}")]
        public async Task<ActionResult<PromoResponse>> GetClientsPromos([FromHeader(Name = "Authorization")]string JWT, int id, int promoId)
        {
            var client = RightCredentials(id).Result;
            
            if (client == null)
            {
                return BadRequest();
            }

            var promo = _context.Promos.Include(p => p.Clients)
                .Where(p => p.Id == id && p.Clients.Contains(client)).FirstOrDefaultAsync();

            if (promo.Result == null)
            {
                return NotFound();
            }

            /*
            if (!client.Promos.Contains(promo))
            {return NotFound();}*/

            return _mapper.Map<Promo, PromoResponse>(promo.Result);

        }

        // PUT: api/ClientConroller/5
        //[Authorize(Roles = Models.Client.Role)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient([FromHeader(Name = "Authorization")]string JWT, int id, ClientRequest client)
        {
            
            if (id != client.Id || RightCredentials(id).Result == null)
            {
                return BadRequest();
            }
            
            _context.Entry(_mapper.Map<ClientRequest, Client>(client)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ClientConroller
        /// <summary>
        /// Создано для тестовых целей, не использовать в приложении. Чтобы создать клиента используйте api/auth/register
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ClientResponse>> PostClient(ClientRequest client)
        {
            var clientEntity = _mapper.Map<ClientRequest, Client>(client);
            var card = new Card(){Id = client.CardId};
            _context.Attach(card);
            _context.Attach(clientEntity);
            clientEntity.Card = card;
            _context.Clients.Add(clientEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, _mapper.Map<Client, ClientResponse>(clientEntity));
        }

        // DELETE: api/ClientConroller/5
        //[Authorize(Roles = Models.Client.Role)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromHeader(Name = "Authorization")]string JWT, int id)
        {
            var client = RightCredentials(id).Result;

            if (client == null)
            {
                return BadRequest();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        private async Task<Client> RightCredentials(int clientId)
        {
            var client =  await _context.Clients
                .FirstOrDefaultAsync(c => /*c.Phone == User.Identity.Name && */c.Id == clientId);

            return client;
        }
    }
}
