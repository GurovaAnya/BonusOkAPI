using System;

namespace BonusOkAPI.Contracts
{
    public class CardResponse
    {
        public int Id { get; set; }
        public int BonusQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CardCode { get; set; }
    }
}