using AutoMapper;
using b1.Data;
using b1.Models;
using b1.ToDo;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using b1.Mappings;

namespace b1.Tests
{
    public class TodoServiceTests
    {
        [Fact]
        public async Task GetAllTodos_ChiTraVeViecCuaDungUserDo()
        {
            // 1. Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TodoTestDb")
                .Options;
            using var context = new AppDbContext(options);

            // Giả lập User 1 có 2 việc, User 2 có 1 việc
            context.TodoItems.AddRange(
                new TodoItem { Id = 1, Title = "Việc của Nam", UserID = 1 },
                new TodoItem { Id = 2, Title = "Việc khác của Nam", UserID = 1 },
                new TodoItem { Id = 3, Title = "Việc của người khác", UserID = 2 }
            );
            await context.SaveChangesAsync();

            // GIẢ LẬP HttpContext: Để GetUserID() trả về 1
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = user };
            mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(httpContext);

            // Các service phụ thuộc khác
            var mockCategory = new Mock<ICategoryService>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile()); // Lấy đúng cấu hình map Entity sang DTO của bạn
            });
            var realMapper = mapperConfig.CreateMapper();

            var todoService = new TodoService(context, mockCategory.Object, realMapper, mockHttpContextAccessor.Object);

            // 2. Act
            var result = await todoService.GetAllTodosDtoAsync();

            // 3. Assert: Dù DB có 3 việc, nhưng chỉ được trả về 2 việc của User 1
            Assert.Equal(2, result.Count);
        }
    }
}
