

using b1.Data;

namespace b1.Controllers
{
    public class ToDoController : ControllerBase
    {
        private readonly ITodoService _itodoService;
        private readonly AppDbContext _appDbContext;
        public ToDoController(ITodoService itodoService, AppDbContext appDbContext)
        {
            _itodoService = itodoService;
            _appDbContext = appDbContext;
        }
        /// <summary>
        /// Them cong viec moi vao danh sach viec can lam
        /// </summary>s
        /// <returns></returns>
        [HttpPost("add-todo")]
        public async Task<ActionResult<TodoItem>> AddTodo(TodoCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Title cannot be null");
            }
            var item = new TodoItem
            {
                Title = dto.Title,
                CategoryId = dto.CategoryId
            };
            var CreatedItem = await _itodoService.AddTodoAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = CreatedItem.Id }, CreatedItem);

        }
        /// <summary>
        /// Hien thi danh sach tat ca cong viec can lam
        /// </summary>
        /// <returns></returns>
        [HttpGet("todoslist")]
        public async Task<List<ToDoGetDto>> GetallToDo()
        {
            return await _appDbContext.TodoItems
                .Select(t => new ToDoGetDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category != null ? t.Category.NameCategory : "Không có"

                })
                .ToListAsync();
        }
        /// <summary>
        /// danh dau cong viec da lam xong theo Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("Update_Todo")]
        public async Task<IActionResult> UpdateToDo(int Id)
        {
            await _itodoService.MarkComple(Id);
            return Ok($"Updated todo with Id: {Id}");

        }
        /// <summary>
        /// Xoa cong viec khoi danh sach theo Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Delete_toDo")]
        public async Task<IActionResult> DeleteToDo(int Id)
        {
            await _itodoService.DeleteToDo(Id);
            return Ok($"Deleted todo with Id: {Id}");
        }
        /// <summary>
        /// tìm kiếm theo id công việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _itodoService.FinByIdDtoAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        /// <summary>
        ///lọc cac công việc theo danh mục
        ///</summary>
        [HttpGet("category")]
        public async Task<ActionResult<List<ToDoGetDto>>> GetByCategorybyId(int categoryId)
        {
            var items = await _itodoService.GetByCategoryIdDtoAsync(categoryId);
            if (items == null || items.Count == 0)
            {
                return NotFound($"No todo items found for categoryId: {categoryId}");
            }
            return Ok(items);
        }
        /// <summary>
        /// hiển thị danh sách theo trang
        /// </summary>
        [HttpGet("pagination")]
        public async Task<ActionResult<List<ToDoGetDto>>> GetPaged(
            [FromQuery] string? search,
            [FromQuery] string? sortBy = "id",
            [FromQuery] bool isDesc = false,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var items = await _itodoService.GetPagedTodosAsync(search, sortBy, isDesc, page, size);
            return Ok(items);
        }

        ///<summary>
        ///test nhiều dữ liệu
        ///</summary>
        [HttpPost("seed-pro")]
        public async Task<IActionResult> SeedPro(int count = 50, int categoryId = 1)
        {
            var faker = new Bogus.Faker<TodoItem>()
                .RuleFor(t => t.Title, f => f.Lorem.Sentence(3)) // Tạo câu 3 từ ngẫu nhiên
                .RuleFor(t => t.IsCompleted, f => f.Random.Bool()) // Random đúng/sai
                .RuleFor(t => t.CategoryId, categoryId)
;

            var items = faker.Generate(count); // Tạo ra 50 đối tượng

            _appDbContext.TodoItems.AddRange(items);
            await _appDbContext.SaveChangesAsync();

            return Ok($"Đã bơm {count} dữ liệu 'như thật' vào Database!");
        }
    }
}
