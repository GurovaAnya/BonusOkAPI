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
    public class CardController : ControllerBase
    {
        private readonly BonusOkContext _context;
        private readonly IMapper _mapper;

        public CardController(BonusOkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Card
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardResponse>>> GetCards()
        {
            var data = await _context.Cards.ToListAsync();
            return  Ok(_mapper.Map<IEnumerable<Card>, IEnumerable<CardResponse>>(data));
        }

        // GET: api/Card/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CardResponse>> GetCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);

            if (card == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Card, CardResponse>(card));
        }

        // PUT: api/Card/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard(int id, CardRequest card)
        {
            if (id != card.Id)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<CardRequest, Card>(card)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardExists(id))
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CardResponse>> PostCard(CardRequest card)
        {
            var cardEntity = _mapper.Map<CardRequest, Card>(card);
            cardEntity.CardCode = Card.GenerateCode();
            _context.Cards.Add(cardEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCard", new { id = card.Id }, _mapper.Map<Card, CardResponse>(cardEntity));
        }

        // DELETE: api/Card/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);
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
        
        [HttpGet("test")]
        public ActionResult<String> Work()
        {
            return "Hello world";
        }
    }
}
