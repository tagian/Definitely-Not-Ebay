using AutoMapper;
using DefNotEbay_API.DTOs.User;
using DefNotEbay_API.Models;

namespace DefNotEbay_API.Mappings
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<CreateUserRequest, User>();
        }
    }
}
