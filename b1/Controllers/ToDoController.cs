

using AutoMapper;
using AutoMapper.QueryableExtensions;
using b1.Data;
using b1.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;

namespace b1.Controllers
{
    [Authorize]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ITodoService _itodoService;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        public ToDoController(ITodoService itodoService, AppDbContext appDbContext, IMapper mapper)
        {
            _itodoService = itodoService;
            _appDbContext = appDbContext;
            _mapper=mapper;
        }
        /// <summary>
        /// Them cong viec moi vao danh sach viec can lam
        /// </summary>s
        /// <returns></returns>
        [HttpPost("add-todo")]
        public async Task<ActionResult<TodoItem>> AddTodo(TodoCreateDto dto)
        {
            //if (dto == null)
            //{
            //    return BadRequest("Title cannot be null");
            //}
            var item = _mapper.Map<TodoItem>(dto);
            var CreatedItem = await _itodoService.AddTodoAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = CreatedItem.Id },
        ApiResponse<TodoItem>.SuccessResponse(CreatedItem, "Thêm mới thành công"));


        }
        /// <summary>
        /// Hien thi danh sach tat ca cong viec can lam
        /// </summary>
        /// <returns></returns>
        [HttpGet("todoslist")]
        public async Task<IActionResult> GetallToDo()
        {
            //var data= await _appDbContext.TodoItems
            //    .ProjectTo<ToDoGetDto>(_mapper.ConfigurationProvider)
            //    .ToListAsync();
            var data= await _itodoService.GetAllTodosDtoAsync();
            return Ok(ApiResponse<List<ToDoGetDto>>.SuccessResponse(data, "Lấy danh sách thành công"));

        }
        /// <summary>
        /// danh dau cong viec da lam xong theo Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("Update_Todo")]
        public async Task<IActionResult> UpdateToDo(int Id)
        {
            var da = await _itodoService.MarkComple(Id);
                
            return Ok(ApiResponse<ToDoGetDto>.SuccessResponse(da, "Sửa đổi thành công"));

        }
        /// <summary>
        /// Xoa cong viec khoi danh sach theo Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Delete_toDo")]
        public async Task<IActionResult> DeleteToDo(int Id)
        {
            var aa = await _itodoService.DeleteToDo(Id);
            return Ok(ApiResponse<ToDoGetDto>.SuccessResponse(aa, $"xóa thành công ToDo có {Id}"));
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
            return Ok(ApiResponse<ToDoGetDto>.SuccessResponse(item, $"tìm thấy To do co {id}"));
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
                throw new KeyNotFoundException("không tìm thấy to nào trong category này");
            }
            return Ok(ApiResponse<List<ToDoGetDto>>.SuccessResponse(items,"tìm thấy các công việc sau"));
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
            return Ok(ApiResponse<List<ToDoGetDto>>.SuccessResponse(items, $"Page {page}"));
        }

        ///<summary>
        ///test nhiều dữ liệu
        ///</summary>
        [HttpPost("seed-pro")]
        public async Task<IActionResult> SeedPro(int count = 50, int categoryId = 1)
        {
            Log.Information("Nam đang thực hiện Seed {Count} dữ liệu cho CategoryId {Id}", count, categoryId);
            var faker = new Bogus.Faker<TodoItem>()
                .RuleFor(t => t.Title, f => f.Lorem.Sentence(3)) // Tạo câu 3 từ ngẫu nhiên
                .RuleFor(t => t.IsCompleted, f => f.Random.Bool()) // Random đúng/sai
                .RuleFor(t => t.CategoryId, categoryId)
;

            var items = faker.Generate(count); // Tạo ra 50 đối tượng

            _appDbContext.TodoItems.AddRange(items);
            await _appDbContext.SaveChangesAsync();

            return Ok(ApiResponse<string>.SuccessResponse($"Đã bơm {count} dữ liệu 'như thật' vào Database!"));
        }
    }
}
