using AutoMapper;
using DefNotEbay_API.DTOs.Bid;
using DefNotEbay_API.Models;


namespace DefNotEbay_API.Mappings
{
    public class BidProfile : Profile
    {
        public BidProfile()
        {
            CreateMap<CreateBidRequest, Bid>();
            CreateMap<Bid, BidResponse>();
        }
    }
}
