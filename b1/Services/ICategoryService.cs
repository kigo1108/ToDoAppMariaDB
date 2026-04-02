

namespace b1.ToDo
{
    public interface ICategoryService
    {
       Task<Category> AddCategoryAsync (string name);
       Task<bool> CategoryExistsAsync(int? CategoryID);
       Task<List<CategoryGetDto>> GetAllCategoriesAsync();
       Task DeleteCategoryAsync(int Id);
    }
}
