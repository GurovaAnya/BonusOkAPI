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
    /// <response code="400"> У клиента нет карты </response>
    /// <response code="403"> Токен неправильный </response>
    /// <response code="404"> Клиент не найден </response>
    [Route("api/Client/{clientId}/Card")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class CardController : ControllerBase
    {
        private readonly BonusOkContext _context;
        private readonly IMapper _mapper;

        public CardController(BonusOkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /*// GET: api/Client/1/Card
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CardResponse>>> GetCards( )
        {
            var data = await _context.Cards.ToListAsync();
            return  Ok(_mapper.Map<IEnumerable<Card>, IEnumerable<CardResponse>>(data));
        }*/

        // GET: api/CLient/2/Card
        [HttpGet]
        //[Authorize(Roles = Models.Client.Role)]
        public async Task<ActionResult<CardResponse>> GetCard([FromHeader(Name = "Authorization")]string JWT, int clientId)
        {
            var client = RightCredentials(clientId).Result;
            if (client == null)
                return BadRequest();
            

            if (client.Card == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Card, CardResponse>(client.Card));
        }

        // PUT: api/Client/2/Card/5
        /// <summary>
        /// Наверное, это тоже стоит убрать потом, потому что клиент обнаглеет от возможности изменить свой бонусный счет
        /// </summary>
        /// <param name="JWT"></param>
        /// <param name="card"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpPut]
        //[Authorize(Roles = Models.Client.Role)]
        public async Task<IActionResult> PutCard([FromHeader(Name = "Authorization")]string JWT, CardRequest card, int clientId)
        {

            var client = await RightCredentials(clientId);

            if (client == null)
                return BadRequest();

            var cardEntity = client.Card;

            if (cardEntity == null)
                return NotFound();

            cardEntity.StartDate = card.StartDate;
            cardEntity.EndDate = card.EndDate;
            cardEntity.BonusQuantity = card.BonusQuantity;
            
            _context.Entry(cardEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardExists(cardEntity.Id))
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

        // POST: api/Card
        [HttpPost]
        //[Authorize(Roles = Models.Client.Role)]
        public async Task<ActionResult<CardResponse>> PostCard([FromHeader(Name = "Authorization")]string JWT, CardRequest card, int clientId)
        {
            var client = await RightCredentials(clientId);
            if (client == null)
                return BadRequest();
            client.CardId = clientId;
            var cardEntity = _mapper.Map<CardRequest, Card>(card);
            cardEntity.CardCode = Card.GenerateCode();
            _context.Cards.Add(cardEntity);
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCard", new { id = client.CardId }, _mapper.Map<Card, CardResponse>(cardEntity));
        }

        // DELETE: api/Card/5
        [HttpDelete]
        //[Authorize(Roles = Models.Client.Role)]
        public async Task<IActionResult> DeleteCard([FromHeader(Name = "Authorization")]string JWT, int clientId)
        {
            var client = await RightCredentials(clientId);
            if (client == null)
                return BadRequest();
            
            var card = client.Card;
            if (card == null)
            {
                return NotFound();
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.Id == id);
        }

        private async Task<Client> RightCredentials(int clientId)
        {
            var client =  await _context.Clients.Include(c => c.Card)
                .FirstOrDefaultAsync(c => /*c.Phone == User.Identity.Name && */c.Id == clientId);

            return client;
        }
    }
}
