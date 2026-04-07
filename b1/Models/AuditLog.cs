using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace b1.Models
{
    public class AuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Action { get; set; } =string.Empty;
        public string Details { get; set; } =string.Empty;
        public DateTime CreatedAt { get; set; } =DateTime.UtcNow;   
    }
}
