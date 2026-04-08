using b1.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace b1.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly AuditLogService _auditLogService;


        public AuthService(AppDbContext appDbContext, IConfiguration configuration, AuditLogService auditLogService)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
            _auditLogService = auditLogService;
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
        //public async Task<string?> Login(UserDto user)
        //{
        //    var newUser = await _appDbContext.Users.FirstOrDefaultAsync(
        //        t => t.userName.ToLower() == user.Username.ToLower());

          
        //    if(newUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, newUser.PasswordHash)){
        //        return null;
        //    }
        //    await _auditLogService.WriteLogAsync("Login", $"Người dùng {user.Username} đã đăng nhập thành công");
        //    return CreateToken(newUser, _configuration);

        //}
        public async Task<TokenResponseDto?> Login(UserDto user)
        {
            var newUser = await _appDbContext.Users.FirstOrDefaultAsync(
                t => t.userName.ToLower() == user.Username.ToLower());


            if (newUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, newUser.PasswordHash))
            {
                return null;
            }
            var token = CreateToken(newUser, _configuration);
            var refreshToken = GenerateRefreshToken();

            //luu Refresh Token vao Db
            newUser.RefreshToken = refreshToken.Token;
            newUser.Tokencreated = refreshToken.TokenCreated;
            newUser.TokenExpires = refreshToken.TokenExpires;
            await _appDbContext.SaveChangesAsync();

            return new TokenResponseDto { Accesstoken = token, RefreshToken = refreshToken.Token };
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                TokenExpires = DateTime.Now.AddDays(7),
                TokenCreated = DateTime.Now
            };
        }

        public async Task<TokenResponseDto?> RefreshToken(string Token)
        {
            var User = await _appDbContext.Users.FirstOrDefaultAsync(
                t => t.RefreshToken == Token);
            if (User == null || User.TokenExpires < DateTime.Now)
            {
                return null;
            }
            var newAccessToken = CreateToken(User, _configuration);
            var newRefreshToken = GenerateRefreshToken();

            User.RefreshToken = newRefreshToken.Token;
            User.Tokencreated = newRefreshToken.TokenCreated;
            User.TokenExpires = newRefreshToken.TokenExpires;

            await _appDbContext.SaveChangesAsync();
            return new TokenResponseDto{Accesstoken = newAccessToken, RefreshToken = newRefreshToken.Token};

        }
        //xóa token khi đăng xuất
        public async Task<bool> RevokeToken(int Id)
        {
            var user= await _appDbContext.Users.FirstOrDefaultAsync(t => t.Id == Id);
            if (user == null)
            {
                return false;
            }
            user.RefreshToken=string.Empty;
            user.Tokencreated = DateTime.MinValue;
            user.TokenExpires = DateTime.MinValue;

            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
