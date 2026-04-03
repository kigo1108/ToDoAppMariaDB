using AutoMapper;
namespace b1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TodoItem, ToDoGetDto>().
                ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.NameCategory : "Không có"));
            CreateMap<TodoCreateDto, TodoItem>();
            CreateMap<Category, CategoryGetDto>();
            CreateMap<CategoryCreateDto, Category>();
        }
    }
}
        