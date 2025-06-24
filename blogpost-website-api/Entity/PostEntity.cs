using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace blogpost_website_api.Entity
{
    public class PostEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserID { get; set; }
        public string CategoryID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Views { get; set; } = 0;
        public int Likes { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> TagID { get; set; } = new();
        public List<string> CommentID { get; set; } = new();
    }
}
