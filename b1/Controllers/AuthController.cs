using b1.Data;
using b1.Services;
using b1.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace b1.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthService authService, IConfiguration configuration, AppDbContext appDbContext)
        {
            _authService = authService;
            _configuration = configuration;
            _appDbContext = appDbContext;
        }
        /// <summary>
        /// Đăng kí 1 User mới
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto user)
        {
            if (await _authService.UserExists(user))
            {
                throw new Exception("đã có User này trong hệ thống");
            }
            _authService.CreateUser(user);
            return Ok(ApiResponse<string>.SuccessResponse(user.Username, "Đăng kí thành công User mới"));
        }
        ///<summary>
        /// Đăng nhập và tạo 1 token
        ///</summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto user)
        {
            var token = await _authService.Login(user);
            if (token == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Sai tài khoản hoặc mật khẩu" }, "thất bại"));
            }
            return Ok(ApiResponse<string>.SuccessResponse(token,"đăng nhập thành công"));
        }
    }
}