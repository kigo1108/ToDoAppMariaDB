using b1.Models;

namespace b1.ToDo
{
    public interface ITodoService
    {

        Task <ToDoGetDto>DeleteToDo(int Id);
        Task <ToDoGetDto> MarkComple(int Id);
        Task<TodoItem> AddTodoAsync(TodoItem? item);
        Task<ToDoGetDto> FinByIdDtoAsync(int Id);
        Task<List<ToDoGetDto>> GetAllTodosDtoAsync(); // Trả về DTO
        Task<List<ToDoGetDto>> GetByCategoryIdDtoAsync(int categoryId); // Trả về DTO

        //xắp xếp, phân trang, tìm kiếm 
        Task<List<ToDoGetDto>> GetPagedTodosAsync(
        string? searchTerm,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize);
    }
}
