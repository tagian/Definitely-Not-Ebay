using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DefNotEbay_API.DTOs.Export
{
    [XmlRoot("Items")]
    public class ExportItemsDto
    {
        [XmlElement("Item")]
        [JsonPropertyName("Item")]
        public List<ExportItemDto> Items { get; set; } = new();
    }

    public  class ExportItemDto
    {
        [XmlAttribute("ItemID")]
        [JsonPropertyName("ItemID")]
        public string ItemID { get; set; } = "";

        [XmlElement("Name")]
        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [XmlElement("Category")]
        [JsonPropertyName("Category")]
        public List<string> Categories { get; set; } = new();

        [XmlElement("Currently")]
        [JsonPropertyName("Currently")]
        public string? Currently { get; set; } 

        [XmlElement("Buy_Price")]
        [JsonPropertyName("Buy_Price")]
        public string? BuyPrice { get; set; } 

        [XmlElement("First_Bid")]
        [JsonPropertyName("First_Bid")]
        public string? FirstBid { get; set; }

        [XmlElement("Number_of_Bids")]
        [JsonPropertyName("Number_of_Bids")]
        public int NumberOfBids { get; set; }

        [XmlArray("Bids")]
        [XmlArrayItem("Bid")]
        [JsonPropertyName("Bids")]
        public List<ExportBidDto> Bids { get; set; } = new();

        [XmlElement("Location")]
        [JsonPropertyName("Location")]
        public ExportLocationDto? Location { get; set; }

        [XmlElement("Country")]
        [JsonPropertyName("Country")]
        public string? Country { get; set; }

        [XmlElement("Started")]
        [JsonPropertyName("Started")]
        public string? Started { get; set; }

        [XmlElement("Ends")]
        [JsonPropertyName("Ends")]
        public string? Ends { get; set; }

        [XmlElement("Seller")]
        [JsonPropertyName("Seller")]
        public ExportSellerDto Seller { get; set; } = new();

        [XmlElement("Description")]
        [JsonPropertyName("Description")]
        public string? Description { get; set; }
    }

    public class ExportBidDto
    {
        [XmlElement("Bidder")]
        [JsonPropertyName("Bidder")]
        public ExportBidderDto Bidder { get; set; } = new();

        [XmlElement("Time")]
        [JsonPropertyName("Time")]
        public string? Time { get; set; }

        [XmlElement("Amount")]
        [JsonPropertyName("Amount")]
        public string? Amount { get; set; }
    }


    public class ExportBidderDto
    {
        [XmlAttribute("UserID")]
        [JsonPropertyName("UserID")]
        public string? UserID { get; set; }

        [XmlAttribute("Rating")]
        [JsonPropertyName("Rating")]
        public string? Rating { get; set; }

        [XmlElement("Location")]
        [JsonPropertyName("Location")]
        public string? Location { get; set; }

        [XmlElement("Country")]
        [JsonPropertyName("Country")]
        public string? Country { get; set; }
    }

    public class ExportLocationDto
    {
        [XmlAttribute("Latitude")]
        [JsonPropertyName("Latitude")]
        public string? Latitude { get; set; }

        [XmlAttribute("Longitude")]
        [JsonPropertyName("Longitude")]
        public string? Longitude { get; set; }

        [XmlText]
        [JsonPropertyName("Value")]
        public string? Value { get; set; }
    }

    public class ExportSellerDto
    {
        [XmlAttribute("UserID")]
        [JsonPropertyName("UserID")]
        public string? UserID { get; set; }

        [XmlAttribute("Rating")]
        [JsonPropertyName("Rating")]
        public string? Rating { get; set; }
    }

}
