using System.Runtime.CompilerServices;

namespace b1.Extensions
{
    public static class CorsExtensions
    { public static IServiceCollection AddcustomCors (this IServiceCollection services)
        {
            services.AddCors(opsitons =>
            {
                opsitons.AddPolicy("FrontendPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
            return services;
        }
    }
}
