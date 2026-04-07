

namespace b1.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Đăng ký các dịch vụ ứng dụng tại đây
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<AuditLogService>();
            // Đăng ký hàng đợi dưới dạng Singleton (Dùng chung cho toàn bộ ứng dụng)
            services.AddSingleton<BackgroundTaskQueue>();
            
            // Đăng ký Service API
            services.AddSingleton<AuditLogService>();

            // Đăng ký Worker Service chạy ngầm
            services.AddHostedService<LogWorkerService>();
            // Cấu hình FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<TodoCreateDtoValidator>();
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
