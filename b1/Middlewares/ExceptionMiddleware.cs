using b1.Wrappers;
using Serilog;
using System.Net;

namespace b1.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "==> Lỗi xảy ra tại: {Path} | Thông điệp: {Message}",
                  context.Request.Path, ex.Message);

                await HandleExceptionAsync(context, ex);

            }

        }
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var er = ApiResponse<string>.ErrorResponse
                (
                message: "đã xảy ra lỗi hệ thống",
                errors : _env.IsDevelopment() ? new List<string> { ex.Message, ex.StackTrace ?? "" } : null

                );
            var json = System.Text.Json.JsonSerializer.Serialize(er);
            await context.Response.WriteAsync(json);


        }


            
    }
}
