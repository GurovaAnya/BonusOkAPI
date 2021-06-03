using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BonusOkAPI.Models;
using FirebaseAdmin.Messaging;
using Newtonsoft.Json;

namespace BonusOkAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private readonly BonusOkContext _context;
        private readonly string pushUrl = "https://fcm.googleapis.com/fcm/send";

        private string getPushHeaders()
        {
            return "key=" + Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        }
        
        public PushController(BonusOkContext context)
        {
            _context = context;
        }

        // POST: api/Client/1/AddDevice/sometoken
        [HttpPost("Client/{clientId}/AddDevice/{token}")]
        public async Task<ActionResult> AddDevice(int clientId, string token)
        {
            var device = new Device()
            {
                ClientId = clientId,
                Token = token
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        // POST: api/SendPush/
        [HttpPost("SendPush")]

        public async Task<ActionResult> SendPushToAll(string title, string messageText)
        {
            var registrationIds = _context.Devices.Select(d => d.Token).AsEnumerable().ToList();
            /*var body = new {registration_ids, message};
            var request = WebRequest.Create(pushUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", getPushHeaders());
            string JsonData = JsonConvert.SerializeObject(body);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(JsonData);
            request.ContentLength = byteArray.Length;
            System.IO.Stream dataStream = await request.GetRequestStreamAsync();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
            string output = "";
            using (System.IO.StreamReader reponse = new System.IO.StreamReader(httpResponse.GetResponseStream()))
            {
                output = reponse.ReadToEnd();
            }

            httpResponse.Close();
            return Ok();*/

            var message = new MulticastMessage()
                {
                    Data = new Dictionary<string, string>()
                    {
                        {"title", title},
                        {"message", messageText},
                    },
                    Tokens = registrationIds,
                };
            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            
            return Ok(response);

        }
    }
}
