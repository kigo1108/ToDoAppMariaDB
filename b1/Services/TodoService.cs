using b1.Data;
using b1.Models;
using b1.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace b1.ToDo
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ICategoryService _categoryService;
        public TodoService(AppDbContext appDbContext, ICategoryService category)
        {
            _appDbContext = appDbContext;
            _categoryService = category;
        }
        public async Task<TodoItem> AddTodoAsync(TodoItem? item)
        {
            var CategoryExist = await _categoryService.CategoryExistsAsync(item.CategoryId);
            if (!CategoryExist)
            {
                throw new ArgumentException($"Danh mục với ID {item.CategoryId} không tồn tại.");
            }
            item.IsCompleted = false;
            _appDbContext.Add(item);
            await _appDbContext.SaveChangesAsync();
            return item;
        }
        public async Task<TodoItem?> FindById(int Id)
        {
            var todoItem = await _appDbContext.TodoItems.FindAsync(Id);
            if (todoItem == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy ToDo với ID {Id}");
            }
            return todoItem;
        }
        //xóa todo
        public async Task<ToDoGetDto> DeleteToDo(int Id)
        {
            var dtoTOdo = await FinByIdDtoAsync(Id);
            var todo = await FindById(Id);
            _appDbContext.TodoItems.Remove(todo);
            await _appDbContext.SaveChangesAsync();
            return dtoTOdo;
        }

        public async Task<List<ToDoGetDto>> GetAllTodosDtoAsync()
        {
            // Chúng ta truyền cả DbSet vào hàm Map, SQL sẽ chỉ SELECT những cột có trong DTO
            return await MapTodoToDto(_appDbContext.TodoItems).ToListAsync();
        }
        //hàm công thức cách mapping từ TodoItem sang ToDoGetDto
        private IQueryable<ToDoGetDto> MapTodoToDto(IQueryable<TodoItem> query)
        {
            return query.Select(t => new ToDoGetDto
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsCompleted,
                CategoryId = t.CategoryId,
                // Sử dụng toán tử điều kiện để tránh lỗi Null nếu Category chưa được nạp
                CategoryName = t.Category != null ? t.Category.NameCategory : "Không có danh mục"
            });
        }
        //đánh dấu hoàn thành
        public async Task<ToDoGetDto> MarkComple(int Id)
        {
            var todo = await FindById(Id);
            if (todo == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy ToDo với ID {Id}");
            }
            todo.IsCompleted = true;
            _appDbContext.TodoItems.Update(todo);
            await _appDbContext.SaveChangesAsync();
            return await FinByIdDtoAsync(Id);
        }
        //find by categoryId trả về DTO
        public async Task<List<ToDoGetDto>> GetByCategoryIdDtoAsync(int categoryId)
        {
            var query = _appDbContext.TodoItems.Where(t => t.CategoryId == categoryId);
            return await MapTodoToDto(query).ToListAsync();
        }

        //find by id trả về DTO
        public async Task<ToDoGetDto> FinByIdDtoAsync(int id)
        {
            var query = _appDbContext.TodoItems.Where(t => t.Id == id);
            return await MapTodoToDto(query).SingleOrDefaultAsync() ?? throw new ArgumentException($"Không tìm thấy ToDo với ID {id}");
        }

        //public async Task<List<ToDoGetDto>> GetPagedTodosAsync(int pageNumber, int pageSize)
        //{
        //    if (pageNumber < 1)
        //    {
        //        pageNumber = 1;
        //    }
        //    var query = _appDbContext.TodoItems.AsQueryable();
        //    var dtoQuery = MapTodoToDto(query);
        //    return await dtoQuery
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //}

        public async Task<List<ToDoGetDto>> GetPagedTodosAsync(string? searchTerm, string? sortBy, bool isDescending, int pageNumber, int pageSize)
        {
            var query = _appDbContext.TodoItems.AsQueryable();

            // 1. Tìm kiếm (Search)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Sử dụng utf8mb4_bin để so sánh chính xác từng byte (phân biệt dấu và hoa thường)
                query = query.Where(t =>
                    EF.Functions.Like(EF.Functions.Collate(t.Title, "utf8mb4_bin"), $"%{searchTerm}%") ||
                    EF.Functions.Like(EF.Functions.Collate(t.Category.NameCategory, "utf8mb4_bin"), $"%{searchTerm}%")
                );
            }
            // 2. Sắp xếp (Sort)
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "title" => isDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                    "id" => isDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
                    _ => query.OrderBy(t => t.Id) // Mặc định sắp xếp theo Id
                };
            }

            // 3. Áp dụng khuôn DTO và Phân trang (Tận dụng hàm Map đã có của Nam)
            var dtoQuery = MapTodoToDto(query);

            var page =await dtoQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if(!page.Any())
            {
                throw new Exception($"không có trang {pageNumber}");
            }
            return page;
        }
    }
}

