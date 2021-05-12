using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonusOkAPI.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        public int BonusQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CardCode { get; set; }
        [InverseProperty(nameof(Models.Client.Card))]

        public virtual Client Client {get; set; }
    }
}