using blogpost_website_api.DB;
using blogpost_website_api.DTO;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using MongoDB.Driver;

namespace blogpost_website_api.Service
{
    public class TagService
    {
        private readonly IMongoCollection<TagEntity> _tag;
        private readonly IMongoCollection<PostEntity> _post;
        public TagService(MongoDBContext context) 
        {
            _post = context.GetCollection<PostEntity>("Post");
            _tag = context.GetCollection<TagEntity>("Tags");
        }

        // create tag
        public async Task<respons> CreateTag(TagDTO tagDTO)
        {
            var existing = await _tag.Find(t => t.Name.ToLower() == tagDTO.Name.ToLower()).FirstOrDefaultAsync();
            if (existing != null)
            {
                return new respons(false, "Tag name already exists");
            }
            var tag = new TagEntity{Name = tagDTO.Name};
            await _tag.InsertOneAsync(tag);
            return new respons(true,"tag succesfull created");
        }
        // read all tag
        public async Task<respons> GetAllTags()
        {
            List<TagEntity> tags = await _tag.Find(_ => true).ToListAsync();
            return  tags.Count > 0 
                ? new respons(true,tags)
                : new respons(false,"dont have tags");
        }
        // Get a tag by ID
        public async Task<respons?> GetTagById(string tagId)
        {
            TagEntity tag = await _tag.Find(t => t.Id == tagId).FirstOrDefaultAsync();
            return  tag!= null
                ? new respons(true,tag)
                : new respons(false,"don't have tag");
        }
        // get post' all tags
        public async Task<respons> GetTagsForPost(string postId)
        {
            var post = await _post.Find(p => p.Id == postId).FirstOrDefaultAsync();
            if (post == null)
                return new respons(false, "Post not found");

            if (post.TagID == null || post.TagID.Count == 0)
                return new respons(false, "No tags found for this post");

            var tags = await _tag.Find(t => post.TagID.Contains(t.Id)).ToListAsync();

            return tags.Count > 0
                ? new respons(true, tags)
                : new respons(false, "Tags not found");
        }
        // Update a tag
        public async Task<respons> UpdateTag(string tagId, TagDTO tagDTO)
        {
            var updatedTag = new TagEntity { Name = tagDTO.Name };
            var result = await _tag.ReplaceOneAsync(t => t.Id == tagId, updatedTag);
            return result.ModifiedCount > 0
                ? new respons(true,"update succesfull")
                : new respons(false);
        }
        // Delete a tag and remove its reference from posts
        public async Task<respons> DeleteTag(string tagId)
        {
            // Remove tagId from all posts that have it
            var postFilter = Builders<PostEntity>.Filter.AnyEq(p => p.TagID, tagId);
            var postUpdate = Builders<PostEntity>.Update.Pull(p => p.TagID, tagId);
            await _post.UpdateManyAsync(postFilter, postUpdate);

            // Delete the tag itself
            var result = await _tag.DeleteOneAsync(t => t.Id == tagId);
            return result.DeletedCount > 0 
                ? new respons(true,"sucessfull delete")
                : new respons(false);
        }

        // tags filter
        public async Task<respons> SearchTags(string searchText)
        {
            FilterDefinition<TagEntity> filter;

            if (string.IsNullOrWhiteSpace(searchText) || searchText.Trim() == "#")
            {
                // Return all tags
                filter = Builders<TagEntity>.Filter.Empty;
            }
            else
            {
                // Case-insensitive partial match 
                filter = Builders<TagEntity>.Filter.Regex(t => t.Name,
                    new MongoDB.Bson.BsonRegularExpression(searchText, "i")
                );
            }

            var tags = await _tag.Find(filter).ToListAsync();

            return tags.Count > 0
                ? new respons(true, tags)
                : new respons(false, "No matching tags found");
        }

    }
}
