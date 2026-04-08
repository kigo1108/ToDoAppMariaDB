namespace b1.Models
{
    public class User
    {
        public int Id { get; set; } 
        public String userName { get; set; } =string.Empty;
        public String PasswordHash { get; set; } =string.Empty;
        public String UserRole { get; set; } = "User";
        public string RefreshToken { get; set; }=string.Empty;
        public DateTime Tokencreated { get; set; }
        public DateTime TokenExpires { get; set; }  
        public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}
