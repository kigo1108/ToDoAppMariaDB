using System.Threading.Channels;
namespace b1.Services
{
    public class BackgroundTaskQueue
    {
        private readonly Channel<AuditLog> _queue;

        public BackgroundTaskQueue()
        {
            // Tạo hàng đợi không giới hạn số lượng tin nhắn
            _queue = Channel.CreateUnbounded<AuditLog>();
        }
        // Hàm để API đẩy log vào hàng đợi
        public async ValueTask QueueLogAsync(AuditLog log)
        {
            await _queue.Writer.WriteAsync(log);
        }
        // 
        public async ValueTask<AuditLog> DequeueLogAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }

    }
}
