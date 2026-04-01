

using b1.Data;
using Microsoft.EntityFrameworkCore.Query;

namespace b1.ToDo
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _appDbContext;
        public CategoryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task AddCategoryAsync(string name)
        {
            var newCategory = new Category
            {
                NameCategory = name,
                IsDeleted = false
            };
            _appDbContext.Categories.Add(newCategory);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> CategoryExistsAsync(int? CategoryID)
        {
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
            return await MapTodoToDto(_appDbContext.Categories).ToListAsync();
        }
        private IQueryable<CategoryGetDto> MapTodoToDto(IQueryable<Category> query)
        {
            return query.Select(t => new CategoryGetDto
            {
                Id = t.Id,
                NameCategory = t.NameCategory,
                ToDoItems = t.TodoItems.Select(ti => new ToDoGetDto
                {
                    Id = ti.Id,
                    Title = ti.Title,
                    IsCompleted = ti.IsCompleted
                }).ToList()
            });
        }



    }
}
