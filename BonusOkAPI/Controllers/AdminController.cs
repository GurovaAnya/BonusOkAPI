using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BonusOkAPI.Contracts;
using BonusOkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;

namespace BonusOkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController: ControllerBase
    {
        private BonusOkContext _context;

        public AdminController(BonusOkContext context)
        {
            _context = context;
        }
         
        /// <summary>
        /// Списать бонусы
        /// </summary>
        /// <param name="charge"> Информация о клиенте и админе</param>
        /// <returns></returns>
        // POST: api/Admin/5/ChargeOff
        [Authorize(Roles = Models.Client.Role)]
        [HttpPost("ChargeOff")]
        public async Task<ActionResult> ChargeOff([FromHeader(Name = "Authorization")]string JWT, Charge charge)
        {
            var admin = RightCredentials(charge.AdminId);
            if (admin.Result == null || !admin.Result.IsAdmin)
                return BadRequest("Вы не админ!");

            var client = _context.Cards.Where(c => c.CardCode == charge.CardCode).FirstOrDefaultAsync().Result;

            if (client == null)
                return NotFound("Клиент не найден");

            if (client.BonusQuantity < charge.BonusQuantity)
                return new ContentResult()
                    {StatusCode = (int)HttpStatusCode.MethodNotAllowed, 
                        Content = "Недостаточно бонусов на счете",};

            client.BonusQuantity -= charge.BonusQuantity;
            _context.Entry(client).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        /// <summary>
        /// Начислить бонусы
        /// </summary>
        /// <param name="charge"> Информация о клиенте и админе</param>
        /// <returns></returns>
        // POST: api/Admin/5/ChargeOff
        [Authorize(Roles = Models.Client.Role)]
        [HttpPost("Award")]
        public async Task<ActionResult> Award([FromHeader(Name = "Authorization")]string JWT, Charge charge)
        {
            var admin = RightCredentials(charge.AdminId);
            if (admin.Result == null || !admin.Result.IsAdmin)
                return BadRequest("Вы не админ!");

            var client = _context.Cards.Where(c => c.CardCode == charge.CardCode).FirstOrDefaultAsync().Result;

            if (client == null)
                return NotFound("Клиент не найден");

            client.BonusQuantity += charge.BonusQuantity;
            _context.Entry(client).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        private async Task<Client> RightCredentials(int clientId)
        {
            var client =  await _context.Clients
                .FirstOrDefaultAsync(c => /*c.Phone == User.Identity.Name &&*/ c.Id == clientId);

            return client;
        }
    }
}