namespace b1.Services
{
    public interface IAuthService
    {
        public String CreateToken(User user, IConfiguration configuration);
        Task<bool>UserExists(UserDto user);
        Task<UserDto> CreateUser(UserDto user);
        Task<string?> Login(UserDto user);
    }
}
