//using b1.Data;
//using b1.DTOs;
//using b1.Models;
//using b1.Services;
//using Castle.Core.Configuration;
//using Microsoft.EntityFrameworkCore;
//using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
//using Moq;

//using Xunit;

//namespace b1.Tests
//{
//    public class AuthServicesTest
//    {
//        [Fact]
//        public async Task Login_VoiMatKhauSai_PhaiTraVeNull()
//        {
//            // 1. Arrange: Tạo DbContext chạy trên RAM
//            var options = new DbContextOptionsBuilder<AppDbContext>()
//                .UseInMemoryDatabase(databaseName: "AuthTestDb")
//                .Options;

//            using var context = new AppDbContext(options);

//            // Thêm một user mẫu vào DB ảo (đã băm mật khẩu bằng BCrypt)
//            context.Users.Add(new User
//            {
//                userName = "nam_hanoi",
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword("matkhau123")
//            });
//            await context.SaveChangesAsync();

//            // Giả lập IConfiguration (vì AuthService cần nó để tạo Token)
//            var mockConfig = new Mock<IConfiguration>();
//            mockConfig.Setup(c => c.GetSection("AppSettings:Token").Value).Returns("chuoi_bí_mật_siêu_dài_để_không_lỗi_token");

//            var authService = new AuthService(context, mockConfig.Object);

//            // 2. Act: Thử đăng nhập sai mật khẩu
//            var result = await authService.Login(new UserDto { Username = "nam_hanoi", Password = "sai_mat_khau" });

//            // 3. Assert: Phải trả về null đúng như logic trong code của bạn
//            Assert.Null(result);
//        }
//    }
//}
