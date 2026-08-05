namespace DefNotEbay_API.Models
{
    public class UserRecommendation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public float Score { get; set; }
        public int Rank { get; set; }         
        public DateTime GeneratedAt { get; set; } 
        public string? ModelTag { get; set; } 

    }
}
