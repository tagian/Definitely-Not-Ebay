using System.ComponentModel.DataAnnotations.Schema;

namespace DefNotEbay_API.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int SellerId { get; set; }
        public int BuyerId { get; set; }
        public int ItemId { get; set; }

        [ForeignKey(nameof(SellerId))]
        public User Seller { get; set; } = default!;

        [ForeignKey(nameof(BuyerId))]
        public User Buyer { get; set; } = default!;

        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = default!;

        public bool BuyNow { get; set; }
        public int? AuctionId {get; set;}
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
