using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace blogpost_website_api.Entity
{
    public class UserEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // author, admin, reader
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public List<string> PostID { get; set; } = new();
        public List<string> OTPID { get; set; } = new();
        public List<string> NitificationID { get; set; } = new();
        public List<string> CommentD { get; set; } = new();

    }
}
