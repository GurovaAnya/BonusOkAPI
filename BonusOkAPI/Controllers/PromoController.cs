using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusOkAPI.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BonusOkAPI.Models;

namespace BonusOkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoController : ControllerBase
    {
        private readonly BonusOkContext _context;

        public PromoController(BonusOkContext context)
        {
            _context = context;
        }

        // GET: api/Promo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promo>>> GetPromos()
        {
            return await _context.Promos.ToListAsync();
        }

        // GET: api/Promo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Promo>> GetPromo(int id)
        {
            var promo = await _context.Promos.FindAsync(id);

            if (promo == null)
            {
                return NotFound();
            }

            return promo;
        }

        // PUT: api/Promo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromo(int id, Promo promo)
        {
            if (id != promo.Id)
            {
                return BadRequest();
            }

            _context.Entry(promo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromoExists(id))
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

        // POST: api/Promo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Promo>> PostPromo(Promo promo)
        {
            _context.Promos.Add(promo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPromo", new { id = promo.Id }, promo);
        }

        // DELETE: api/Promo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromo(int id)
        {
            var promo = await _context.Promos.FindAsync(id);
            if (promo == null)
            {
                return NotFound();
            }

            _context.Promos.Remove(promo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromoExists(int id)
        {
            return _context.Promos.Any(e => e.Id == id);
        }
    }
}
