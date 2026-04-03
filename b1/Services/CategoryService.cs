
using AutoMapper;
using AutoMapper.QueryableExtensions;
using b1.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Query;

namespace b1.ToDo
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        public CategoryService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<Category> AddCategoryAsync(CategoryCreateDto CateDto)
        {
            
            var newCategory = _mapper.Map<Category>(CateDto);
            _appDbContext.Categories.Add(newCategory);
            await _appDbContext.SaveChangesAsync();
            return newCategory;
        }

        public async Task<bool> CategoryExistsAsync(int? CategoryID)
        {
            if(CategoryID == null)
            {
                throw new ArgumentException("CategoryID không được để trống");
            }
            return await _appDbContext.Categories.AnyAsync(c => c.Id == CategoryID);
        }

        public async Task DeleteCategoryAsync(int Id)
        {
            var Category = await _appDbContext.Categories.FindAsync(Id);
            if (Category == null)
            {
                throw new ArgumentException("lỗi");
            }
            Category.IsDeleted = true;
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<CategoryGetDto>> GetAllCategoriesAsync()
        {
            //return await MapTodoToDto(_appDbContext.Categories).ToListAsync();
            return await _appDbContext.Categories
                .Where(c => !c.IsDeleted)
                .ProjectTo<CategoryGetDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesNoTodoAsync()
        {
            return await _appDbContext.Categories.Where(c => !c.IsDeleted).ToListAsync();
        }
    }
}
