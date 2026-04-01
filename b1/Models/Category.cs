namespace b1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? NameCategory { get; set; }
        // thêm thuộc tính IsDeleted để đánh dấu khi xóa mềm, mặc định là false
        public bool IsDeleted { get; set; } = false;    
        // tao ra moi quan he 1-n voi TodoItem dung virtual de tao Proxy Design Pattern de dung co che (Lazy Loading) dung ICollection de co the cho TodoItem co the la 1 List hay 1 HashSet
        public virtual ICollection<TodoItem> TodoItems { get; set; } =new List<TodoItem>();
    }
}
