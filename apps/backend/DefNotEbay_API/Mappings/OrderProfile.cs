using AutoMapper;
using DefNotEbay_API.DTOs.Order;
using DefNotEbay_API.DTOs.User;
using DefNotEbay_API.Models;

namespace DefNotEbay_API.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<CreateOrderRequest, Order>();
            CreateMap<Order, OrderResponse>();
        }
    }
}
