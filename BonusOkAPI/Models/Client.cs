using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BonusOkAPI.Models
{
    public class Client 
    {
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
        
        
        [ForeignKey(nameof(CardId))]
        [InverseProperty("Client")]
        public virtual Card Card { get; set; }
        
        [InverseProperty("Clients")]
        public virtual DbSet<Promo> Promos { get; set; }
    }
}