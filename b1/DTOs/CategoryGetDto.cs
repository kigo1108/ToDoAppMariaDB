namespace b1.DTOs
{
    public class CategoryGetDto
    {
        public int Id { get; set; }
        public string NameCategory { get; set; } = string.Empty;
        public List<ToDoGetDto> ToDoItems { get; set; } = new List<ToDoGetDto>();
    }
}
