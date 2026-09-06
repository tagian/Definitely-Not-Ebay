namespace DefNotEbay_API.DTOs.Order
{
    public class CreateOrderRequest
    {
        public int SellerId { get; set; }
        public int BuyerId { get; set; }
        public int ItemId { get; set; }
        public bool BuyNow { get; set; }
        
    }
}
