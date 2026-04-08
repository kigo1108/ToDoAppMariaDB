using AutoMapper;
using b1.Data;
using b1.Services;
using b1.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using System.Security.Claims;

namespace b1.Controllers
{

    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly AuditLogService _auditLogService;
        public AuthController(IAuthService authService, IConfiguration configuration, AppDbContext appDbContext, AuditLogService auditLog)
        {
            _authService = authService;
            _configuration = configuration;
            _appDbContext = appDbContext;
            _auditLogService= auditLog;
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
            await _authService.CreateUser(user);
            return Ok(ApiResponse<string>.SuccessResponse(user.Username, "Đăng kí thành công User mới"));
        }
        ///<summary>
        /// Đăng nhập và tạo 1 token
        ///</summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto user)
        {
            var result = await _authService.Login(user);
            if (result == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Sai tài khoản hoặc mật khẩu" }, "thất bại"));
            }
            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result,"đăng nhập thành công"));
        }

        ///<summary>
        ///xem log 
        ///</summary>
        [HttpGet("system-logs")]
        public async Task<IActionResult> GetSystemLog()
        {
            var logs= await _auditLogService.GetLogsAsync();
            return Ok(ApiResponse<List<AuditLog>>.SuccessResponse(logs, "xong"));
        }

        ///<summary>
        ///Refresh Token va lay AccessToken moi
        ///</summary>
        [HttpGet("Refresh_Token")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            var newToken= await _authService.RefreshToken(token);
            if(newToken == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Token không hợp lệ hoặc đã hết hạn" }, "Vui lòng đăng nhập lại "));
            }
            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(newToken, "đã cập nhật token mới"));
        }

        ///<summary>
        ///Đăng Xuất Và xóa Token
        /// </summary>
        [Authorize]
        [HttpPost("Log_Out")]
        public async Task<IActionResult> LogOut()
        {
            var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await _authService.RevokeToken(int.Parse(userId));
            if (!result)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(new List<string> { "Không thế đăng xuất" }));
            }
            return Ok(ApiResponse<string>.SuccessResponse("đăng xuất thành công", "đã cập nhật token mới"));
        }

    }
}