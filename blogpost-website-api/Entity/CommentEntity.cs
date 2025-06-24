using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace blogpost_website_api.Entity
{
    public class CommentEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
    }
}
