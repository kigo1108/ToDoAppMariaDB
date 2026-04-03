namespace b1.Models
{
    public class User
    {
        public int Id { get; set; } 
        public String userName { get; set; } =string.Empty;
        public String PasswordHash { get; set; } =string.Empty;
    }
}
