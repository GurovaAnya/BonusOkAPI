namespace BonusOkAPI.Contracts
{
    public class PromoRequestWithImage:PromoRequest
    {
        public byte[] Image { get; set; }
    }
}