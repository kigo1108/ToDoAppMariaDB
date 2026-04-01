using b1.Data;
using b1.Models;
using Microsoft.EntityFrameworkCore;

namespace b1.ToDo
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ICategoryService _categoryService;
        public TodoService(AppDbContext appDbContext,ICategoryService category)
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
            if(todoItem == null)
            {
                return null;
            }
            return todoItem;
        }

        public async Task DeleteToDo(int Id)
        {
            var todo = await FindById(Id);
            if(todo == null)
            {
                return;
            }
            _appDbContext.TodoItems.Remove(todo);
            await _appDbContext.SaveChangesAsync();
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
        public async Task MarkComple(int Id)
        {
            var todo = await FindById(Id);
            if(todo == null)
            {
                return;
            }
            todo.IsCompleted = true;
            _appDbContext.TodoItems.Update(todo);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<List<ToDoGetDto>> GetByCategoryIdDtoAsync(int categoryId)
        {
            var query = _appDbContext.TodoItems.Where(t => t.CategoryId == categoryId);
            return await MapTodoToDto(query).ToListAsync();
        }
        public async Task<ToDoGetDto> FinByIdDtoAsync(int id)
        {
            var query = _appDbContext.TodoItems.Where(t => t.Id == id);
            return await MapTodoToDto(query).SingleOrDefaultAsync() ?? throw new ArgumentException($"Không tìm thấy ToDo với ID {id}");
        }

        public async Task<List<ToDoGetDto>> GetPagedTodosAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var query = _appDbContext.TodoItems.AsQueryable();
            var dtoQuery = MapTodoToDto(query);
            return await dtoQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        }
    }

