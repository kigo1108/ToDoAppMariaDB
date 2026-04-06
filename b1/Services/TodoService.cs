using AutoMapper;
using AutoMapper.QueryableExtensions;
using b1.Data;
using b1.Models;
using b1.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace b1.ToDo
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TodoService(AppDbContext appDbContext, ICategoryService category, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _categoryService = category;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ToDoGetDto?> AddTodoAsync(TodoCreateDto? dto)
        {
            var CategoryExist = await _categoryService.CategoryExistsAsync(dto.CategoryId);
            if (!CategoryExist)
            {
                return null;
            }
            var item = _mapper.Map<TodoItem>(dto);
            item.UserID = GetUserID();   
            item.IsCompleted = false;
            _appDbContext.TodoItems.Add(item);
            await _appDbContext.SaveChangesAsync();


            return _mapper.Map<ToDoGetDto>(item);
        }
       
        //xóa todo
        public async Task<ToDoGetDto> DeleteToDo(int Id)
        {
            var toDoItem= await FindById(Id);
            if (toDoItem == null)
            {
                return null;
            }
            _appDbContext.TodoItems.Remove(toDoItem);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<ToDoGetDto>(toDoItem);
        }

        public async Task<List<ToDoGetDto>> GetAllTodosDtoAsync()
        {
            var currentUserId=GetUserID();
            return await _appDbContext.TodoItems
                .Where(t => t.UserID == currentUserId)
                .ProjectTo<ToDoGetDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        
        public async Task<ToDoGetDto> MarkComple(int Id)
        {
            var todo = await FindById(Id);
            if (todo == null)
            {
                return null ;
            }
            todo.IsCompleted = true;
            _appDbContext.TodoItems.Update(todo);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<ToDoGetDto>(todo);
        }
        //find by categoryId trả về DTO
        public async Task<List<ToDoGetDto>> GetByCategoryIdDtoAsync(int categoryId)
        {
            var currentUserId = GetUserID();
            return await _appDbContext.TodoItems
                .Where(t => t.CategoryId == categoryId&& t.UserID==currentUserId)
                .ProjectTo<ToDoGetDto>(_mapper.ConfigurationProvider)
                .ToListAsync() ;
        }

        //find by id trả về DTO
        public async Task<ToDoGetDto?> FinByIdDtoAsync(int id)
        {
            var currentUser=GetUserID();
            var item = await _appDbContext.TodoItems
                .Where (t => t.Id == id&& t.UserID==currentUser)
                .ProjectTo<ToDoGetDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return item;
        }

        
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
            //var dtoQuery = MapTodoToDto(query);


            //var pageItems =await query
            //    .Include(t=>t.Category)
            //    .Skip((pageNumber - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToListAsync();
            var result = await query
                .ProjectTo<ToDoGetDto>(_mapper.ConfigurationProvider)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (!result.Any())
            {
                return null;
            }

            return result;
        }

        //Lay UserId
        private int GetUserID()
        {
            var userID=_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userID ?? "0");
        }
        public async Task<TodoItem?> FindById(int Id)
        {
            var currentUserId = GetUserID();
            return await _appDbContext.TodoItems
                .FirstOrDefaultAsync(t => t.Id == Id && t.UserID == currentUserId);

        }
    }
}

