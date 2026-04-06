

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

            var createdItem = await _itodoService.AddTodoAsync(dto);
            if (createdItem == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Không tìm thấy Category" }, "That bai"));
            }
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id },
        ApiResponse<ToDoGetDto>.SuccessResponse(createdItem, "Thêm mới thành công"));


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
            if (data == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "bạn chưa có công việc nào" }, "Thất Bại"));
            }
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
            if (da == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "không có công việc hoặc bạn ko có quyền đánh dấu" }, "Thất Bại"));
            }

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
            if (aa == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Không tìm thấy hoặc bạn không có quyền sửa." }, "Thất Bại"));
            }
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
            if (item == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Không tìm thấy hoặc bạn không có quyền xem." }, "Thất Bại"));
            }
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
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Không tìm thấy các công việc trong category id này của bạn" }, "Thất Bại"));
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
            if (items == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> {$"không có dữ liệu ở trang {page}" }, "Thất Bại"));
            }
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
