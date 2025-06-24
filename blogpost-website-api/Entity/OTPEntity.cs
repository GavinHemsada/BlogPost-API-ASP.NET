using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace blogpost_website_api.Entity
{
    public class OTPEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserID { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
