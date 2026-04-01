namespace b1.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool IsCompleted { get; set; }

        //foreign key lien ket voi Category
        public int? CategoryId { get; set; }

        //Reference Navigation Property: Đối tượng Category tương ứng
        // Sử dụng 'virtual' để đồng bộ với Category.cs
        // '?' để đánh dấu có thể null nếu bạn chưa gán Category ngay lập tức
        public virtual Category? Category { get; set; }
    }
}
