using MongoDB.Driver;
using b1.Data;
namespace b1.Services
{
    public class AuditLogService
    {
        private readonly IMongoCollection<AuditLog> _log;
        public AuditLogService(IConfiguration config)
        {
            var Client = new MongoClient(config.GetConnectionString("MongoConnection"));
            var Database = Client.GetDatabase("TodoAuđitb");
            _log = Database.GetCollection<AuditLog>("Logs");
        }
        
        public async Task WriteLog(String action, String detail)
        {
            await _log.InsertOneAsync(new AuditLog { Action = action, Details = detail });
        }
        public async Task<List<AuditLog>> GetLogsAsync()
        {
            return await _log.Find(_ => true)
                              .SortByDescending(l => l.CreatedAt)
                              .Limit(50) // Lấy 50 bản ghi mới nhất
                              .ToListAsync();
        }
    }
}
