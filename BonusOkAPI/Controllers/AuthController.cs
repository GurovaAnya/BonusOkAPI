using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BonusOkAPI.Models;
using BonusOkAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BonusOkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BonusOkContext _context;
        private readonly IMapper _mapper;

        public AuthController(BonusOkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Token(string number, int code)
        {
            var identity = GetIdentity(number, code);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or code." });
            }
 
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            
            var client = await _context.Clients.Where(c => c.Phone == number).FirstOrDefaultAsync();
            
            client.AuthCode = null;
            client.AuthCodeDeathTime = null;
            await _context.SaveChangesAsync();
 
            return Ok(response);
        }
        
        private ClaimsIdentity GetIdentity(string phone, int code)
        {
            Client person = _context.Clients.FirstOrDefault(x => x.AuthCode.HasValue && x.Phone == phone && x.AuthCode == code);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, Models.Client.Role),
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
 
            // если пользователя не найдено
            return null;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <response code ="400"> Неправильный формат номера </response>
        /// <response code ="409"> Клиент с таким телефоном уже есть </response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterClient(string number)
        {
            if (!number.All(char.IsDigit))
                return BadRequest("Неправильный формат номера");
            
            var card = new Card()
            {
                BonusQuantity = 0,
                StartDate = DateTime.Now,
            };

            try
            {
                var client = new Client()
                {
                    Phone = number,
                    Card = card
                };

                _context.Clients.Add(client);
                await SaveCode(client);
            }
            catch (SqlException exception)
            {
                return Conflict("Клиент с таким телефоном уже существует");
            }

            return Ok();
        }
                
        [HttpGet("request_code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestAuthCode(string number)
        {
            var client = await _context.Clients.Where(c => c.Phone == number).FirstOrDefaultAsync();
            if (client == null)
                return NotFound();
            
            await SaveCode(client);
            return Ok();
        }

        private async Task SaveCode(Client client)
        {
            Random random = new Random();
            client.AuthCode = random.Next(1000, 10000);
            client.AuthCodeDeathTime = DateTime.Now + new TimeSpan(0, 5, 0);
            await _context.SaveChangesAsync();
        }
        
        
        /// <summary>
        /// Метод для получения кода клиента. Не использовать в приложении!!!!!
        /// </summary>
        /// <param name="number">Номер телефона</param>
        /// <returns> </returns>
        [HttpGet("get_code_secret_request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCode(string number)
        {
            var client = await _context.Clients.Where(c => c.Phone == number).FirstOrDefaultAsync();
            if (client == null)
                return NotFound();
            
            
            return Ok(
                new
                {
                    client.AuthCode,
                    client.AuthCodeDeathTime
                });
        }
        
        [HttpGet("test")]
        public ActionResult<String> Work()
        {
            return "Hello world";
        }
    }
}