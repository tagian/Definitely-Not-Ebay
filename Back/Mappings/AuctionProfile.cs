using AutoMapper;
using DefNotEbay_API.DTOs.Auction;
using DefNotEbay_API.Models;

namespace DefNotEbay_API.Mappings
{
    public class AuctionProfile : Profile
    {
        public AuctionProfile()
        {
            CreateMap<CreateAuctionRequest, Auction>();
            CreateMap<UpdateAuctionRequest, Auction>();
            CreateMap<Auction, AuctionResponse>();
        }
    }
}
