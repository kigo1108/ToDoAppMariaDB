

using b1.Wrappers;

namespace b1.Controllers
{
    [ApiController]
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
        public async Task<IActionResult> AddCategory(CategoryCreateDto categoryCreate)
        {
            
            var cate = await _categoryService.AddCategoryAsync(categoryCreate.NameCategory);
            return Ok(ApiResponse<Category>.SuccessResponse(cate,"thêm thành công"));
        }

        /// <summary>
        /// Xóa Category theo Id
        /// </summary>
        [HttpDelete("Delete_Category")]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            await _categoryService.DeleteCategoryAsync(Id);
            return Ok(ApiResponse<string>.SuccessResponse($"đã xóa thành công Category có {Id}"));
        }

        /// <summary>
        /// hiển thị tất cả danh sách catergory
        /// </summary>
        [HttpGet("GetAllCategory")]
        public async Task<ActionResult<List<CategoryGetDto>>> GetAllCategory()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<List<CategoryGetDto>>.SuccessResponse(categories,"hiện thành công"));
        }
    }
}
