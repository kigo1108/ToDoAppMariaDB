using MongoDB.Driver;

namespace b1.Services
{
    public class LogWorkerService : BackgroundService
    {
        private readonly BackgroundTaskQueue _queue;
        private readonly IMongoCollection<AuditLog> _logs;
        private readonly ILogger<LogWorkerService> _logger;

        public LogWorkerService(BackgroundTaskQueue queue, IConfiguration congfig, ILogger<LogWorkerService> logger)
        {
            _queue = queue;
            _logger = logger;
            //kết nối database MongoDb
            var client = new MongoClient(congfig.GetConnectionString("MongoConnection"));
            var database = client.GetDatabase("TodoAuditDb");
            _logs = database.GetCollection<AuditLog>("UserLogs");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker Service ghi Log đang chạy...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Đợi cho đến khi có tin nhắn trong hàng đợi
                    var log = await _queue.DequeueLogAsync(stoppingToken);

                    // Thực hiện lưu vào MongoDB thực sự
                    await _logs.InsertOneAsync(log, cancellationToken: stoppingToken);

                    _logger.LogInformation($"Đã lưu Log ngầm: {log.Action}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lưu Log ngầm.");
                }
            }
        }
    }
}
