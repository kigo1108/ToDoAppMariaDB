
using b1.Data;
using b1.Middlewares;
using b1.Validators.Category;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;


namespace b1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
             });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<ITodoService, TodoService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            // Thêm cái này để .NET hiểu về các đường dẫn API
            builder.Services.AddEndpointsApiExplorer();

            // Thêm cái này để tạo ra bộ máy phát sinh tài liệu Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            //lay chuoi ket not tu file appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            //dang ky AppDbContext vao DI container va cau hinh ket noi den MySQL
            builder.Services.AddDbContext<AppDbContext>(opsitions =>opsitions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            //đăng kí FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<TodoCreateDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining <CategoryCreateDtoValidator>();
            // 1. Cấu hình Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Ghi từ mức Info trở lên (Info, Warning, Error, Fatal)
                .WriteTo.Console()           // Hiện log đẹp mắt ở màn hình Console
                .WriteTo.File("Logs/todo-api-.txt", rollingInterval: RollingInterval.Day) // Mỗi ngày tạo 1 file log mới trong folder Logs
                .CreateLogger();

            // 2. Thay thế Logger mặc định của .NET bằng Serilog
            builder.Host.UseSerilog();
            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options=>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "api");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
