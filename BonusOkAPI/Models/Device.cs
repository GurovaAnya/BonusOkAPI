using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BonusOkAPI.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public int ClientId { get; set; }
        
        [ForeignKey(nameof(ClientId))]
        public virtual Client Client { get; set; }
    }
}