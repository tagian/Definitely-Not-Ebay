using AutoMapper;
using DefNotEbay_API.Models;
using DefNotEbay_API.DTOs.User;



namespace DefNotEbay_API.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<UpdateUserRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
