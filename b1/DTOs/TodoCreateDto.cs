namespace b1.DTOs
{
    public class TodoCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
