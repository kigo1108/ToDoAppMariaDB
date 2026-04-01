

namespace b1.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        /// <summary>
        /// Them danh muc moi vao danh sach cac danh muc
        /// </summary>
        /// <param name="name"></param>
        [HttpPost("Add_Category")]
        public async Task<IActionResult> AddCategory(string name)
        {
            if (name == null)
            {
                return BadRequest("Name cannot be null");
            }
            await _categoryService.AddCategoryAsync(name);
            return Ok($"Added category: {name}");
        }

        /// <summary>
        /// Xóa Category theo Id
        /// </summary>
        [HttpDelete("Delete_Category")]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            await _categoryService.DeleteCategoryAsync(Id);
            return Ok($"Deleted category with Id: {Id}");
        }

        /// <summary>
        /// hiển thị tất cả danh sách catergory
        /// </summary>
        [HttpGet("GetAllCategory")]
        public async Task<ActionResult<List<CategoryGetDto>>> GetAllCategory()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
    }
}
