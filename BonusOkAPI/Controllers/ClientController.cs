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

namespace BonusOkAPI.Controllers
{
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientResponse>>> GetClients()
        {
            var data = await _context.Clients.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<Client>, IEnumerable<ClientResponse>>(data));
        }

        // GET: api/ClientConroller/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponse>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Client, ClientResponse>(client));
        }
        
        // GET: api/ClientConroller/5/Promo
        [HttpGet("{id}/Promo")]
        public async Task<ActionResult<IEnumerable<PromoResponse>>> GetClientsPromos(int id)
        {
            var data = await _context.Clients
                .Where(c => c.Id == id)
                .Include(c => c.Promos)
                .SelectMany(c => c.Promos)
                .Where(c => c.StartDate >= DateTime.Now)
                .ToArrayAsync();
            return Ok(_mapper.Map<IEnumerable<Promo>, IEnumerable<PromoResponse>>(data));
        }
        
        // GET: api/ClientConroller/5/Promo/1
        [HttpGet("{id}/Promo/{promoId}")]
        public async Task<ActionResult<PromoResponse>> GetClientsPromos(int id, int promoId)
        {
            var promo = await _context.Promos.FindAsync(promoId);
            
            if (promo == null)
            {
                return NotFound();
            }
            
            var client = promo.Clients.FindAsync(id);
            
            if (client.Result == null)
            {return NotFound();}

            return _mapper.Map<Promo, PromoResponse>(promo);

        }

        // PUT: api/ClientConroller/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, ClientRequest client)
        {
            if (id != client.Id)
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(ClientRequest client)
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
