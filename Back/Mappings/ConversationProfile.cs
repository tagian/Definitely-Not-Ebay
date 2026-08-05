using AutoMapper;
using DefNotEbay_API.DTOs.Conversation;
using DefNotEbay_API.Models;

namespace DefNotEbay_API.Mappings
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<CreateConversationRequest, Conversation>();
            CreateMap<Conversation, ConversationResponse>();
        }
    }
}
