using AutoMapper;
using DefNotEbay_API.DTOs.Item;
using DefNotEbay_API.Models;

namespace DefNotEbay_API.Mappings
{
    public class ItemProfile : Profile 
    {
        public ItemProfile()
        {
            CreateMap<CreateItemRequest, Item>();
            CreateMap<UpdateItemRequest, Item>();
            CreateMap<Item, ItemResponse>();

        }
    }
}
