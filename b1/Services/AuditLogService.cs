using MongoDB.Driver;
using b1.Data;
using Microsoft.EntityFrameworkCore.Query;
namespace b1.Services
{
    public class AuditLogService
    {
        private readonly BackgroundTaskQueue _queue;
        private readonly IConfiguration _configuration;
        public AuditLogService(BackgroundTaskQueue queue, IConfiguration configuration)
        {
            _queue = queue;
            _configuration = configuration;
        }

        public async Task WriteLogAsync(String action, string detail)
        {
            var log =new AuditLog {Action =action, Details = detail};
            //đẩy vào hàm đợi và kết thúc
            await _queue.QueueLogAsync(log);
        }
        public async Task<List<AuditLog>> GetLogsAsync  ()
        {
            // Kết nối trực tiếp đến MongoDB để lấy dữ liệu (vì đây là thao tác đọc, không cần qua Worker)
            // Bạn có thể inject IMongoCollection vào constructor hoặc khởi tạo nhanh như sau:
            var connectionString = _configuration.GetConnectionString("MongoConnection");
           var client = new MongoClient(connectionString);
            var database = client.GetDatabase("TodoAuditDb");
            var collection = database.GetCollection<AuditLog>("UserLogs");

            return await collection.Find(_ => true)
                                   .SortByDescending(l => l.CreatedAt)
                                   .Limit(50)
                                   .ToListAsync();
        }
    }
}
