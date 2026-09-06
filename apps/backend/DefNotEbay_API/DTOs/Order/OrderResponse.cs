using System.ComponentModel.DataAnnotations.Schema;

namespace DefNotEbay_API.DTOs.Order
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int SellerId { get; set; }
        public int BuyerId { get; set; }
        public int ItemId { get; set; }
        public bool BuyNow { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
