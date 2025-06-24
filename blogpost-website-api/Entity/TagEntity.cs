using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace blogpost_website_api.Entity
{
    public class TagEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> PostID { get; set; } = new();
    }
}
