using System;

namespace BonusOkAPI.Contracts
{
    public class CardRequest
    {
        public int BonusQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}