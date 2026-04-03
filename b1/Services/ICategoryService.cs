

namespace b1.ToDo
{
    public interface ICategoryService
    {
       Task<Category> AddCategoryAsync (CategoryCreateDto cateDto);
       Task<bool> CategoryExistsAsync(int? CategoryID);
       Task<List<CategoryGetDto>> GetAllCategoriesAsync();
        Task<List<Category>> GetAllCategoriesNoTodoAsync();
       Task DeleteCategoryAsync(int Id);
    }
}
