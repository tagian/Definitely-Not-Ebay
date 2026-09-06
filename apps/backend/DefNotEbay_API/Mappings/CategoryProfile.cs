using AutoMapper;
using DefNotEbay_API.DTOs.Category;
using DefNotEbay_API.Models;


namespace DefNotEbay_API.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();
            CreateMap<Category, CategoryResponse>();

        }
    }
}
