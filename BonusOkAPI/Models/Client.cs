using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BonusOkAPI.Models
{
    public class Client
    {
        public const string Role = "Client";
        [Key]
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        
        public int CardId { get; set; }
        public int? AuthCode { get; set; }
        public DateTime? AuthCodeDeathTime { get; set; }
        
        public bool IsAdmin { get; set; }
        
        [ForeignKey(nameof(CardId))]
        [InverseProperty("Client")]
        public virtual Card Card { get; set; }
        
        [InverseProperty("Clients")]
        public virtual HashSet<Promo> Promos { get; set; }
        
        public virtual HashSet<Device> Devices { get; set; }
    }
}