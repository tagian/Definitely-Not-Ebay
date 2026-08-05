using AutoMapper;
using DefNotEbay_API.DTOs.Message;
using DefNotEbay_API.Models;

namespace DefNotEbay_API.Mappings
{
    public class MessageProfile : Profile {
        public MessageProfile()
        {
            CreateMap<CreateMessageRequest, Message>();
            CreateMap<Message, MessageResponse>();
        }
         
    }
}

