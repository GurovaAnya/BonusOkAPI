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

        public static int GenerateCode()
        {
            var date = DateTime.Now;
            return date.Millisecond * 5053 + date.Second*295 + date.Day * 285 + date.Month * 35 + date.Year * 395;
        }
    }
}