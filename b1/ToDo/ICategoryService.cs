

namespace b1.ToDo
{
    public interface ICategoryService
    {
       Task AddCategoryAsync (string name);
       Task<bool> CategoryExistsAsync(int? CategoryID);
       Task<List<CategoryGetDto>> GetAllCategoriesAsync();
       Task DeleteCategoryAsync(int Id);
    }
}
