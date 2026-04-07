
using b1.Data;
using b1.Extensions;
using b1.Mappings;
using b1.Middlewares;
using b1.Validators.Category;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;


namespace b1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // 1. Cấu hình Serilog (Giữ lại ở đây vì nó tác động đến builder.Host)
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File("Logs/todo-api-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                builder.Host.UseSerilog();

                // 2. Đăng ký các nhóm Service qua Extension Methods
                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    });

                builder.Services.AddApplicationServices();
                builder.Services.AddInfrastructureServices(builder.Configuration);
                builder.Services.AddSwaggerDocumentation();
                builder.Services.AddAutoMapper(typeof(MappingProfile));
                builder.Services.AddAuthentication().AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            builder.Configuration.GetSection("AppSettings:Token").Value!))
                    };
                });
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                    options.InstanceName = "TodoApp_";
                });
                var app = builder.Build();

                // 3. Configure HTTP Request Pipeline
                app.UseMiddleware<ExceptionMiddleware>();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwaggerDocumentation(); // Gọi Extension rút gọn
                }

                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                // Đoạn code này sẽ "moi" chi tiết lỗi ra cho bạn xem
                Console.WriteLine("\n=== LỖI CHI TIẾT TỪ LOADER EXCEPTIONS ===");
                foreach (var loaderEx in ex.LoaderExceptions)
                {
                    if (loaderEx != null)
                    {
                        Console.WriteLine($"- {loaderEx.Message}");
                    }
                }
                Console.WriteLine("==========================================\n");
                throw; // Vẫn throw để dừng chương trình
            }
        }
    }
}