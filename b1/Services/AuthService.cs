using b1.Data;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace b1.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;


        public AuthService(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }
        //tạo token cho user
        public String CreateToken(User user, IConfiguration configuration)
        {
            // tạo claims để lưu thông tin người dùng vào token 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.userName),
                new Claim(ClaimTypes.Role, user.UserRole)
            };
            // lấy Token từ appsettings.jsona
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value!));

            // 3. Ký tên xác nhận
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Tạo Token có hạn sử dụng (ví dụ 1 ngày)
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //tạo 1 user mới và băm mật khẩu
        public async Task<UserDto> CreateUser(UserDto user)
        {
            var newUser = new User
            {
                userName = user.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password)
            };
            _appDbContext.Users.Add(newUser);
            await _appDbContext.SaveChangesAsync();
            return user;
        }
        //check user có tồn tại trong hệ thống ko
        public async Task<bool> UserExists(UserDto user)
        {
            return await _appDbContext.Users.AnyAsync(x => x.userName.ToLower() == user.Username.ToLower());
        }
        //đăng nhập
        public async Task<string?> Login(UserDto user)
        {
            var newUser = await _appDbContext.Users.FirstOrDefaultAsync(
                t => t.userName.ToLower() == user.Username.ToLower());

          
            if(newUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, newUser.PasswordHash)){
                return null;
            }
           return CreateToken(newUser, _configuration);

        }
    }
}
