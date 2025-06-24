using blogpost_website_api.DB;
using blogpost_website_api.DTO;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using blogpost_website_api.Security;
using MongoDB.Driver;

namespace blogpost_website_api.Service
{
    public class PostService
    {
        private readonly IMongoCollection<UserEntity> _users;
        private readonly IMongoCollection<TagEntity> _tag;
        private readonly IMongoCollection<CategoryEntity> _category;
        private readonly IMongoCollection<PostEntity> _post;
        private readonly JWT jwt;

        public PostService(MongoDBContext context, JWT jwt)
        {
            _users = context.GetCollection<UserEntity>("Users");
            _post = context.GetCollection<PostEntity>("Post");
            _tag = context.GetCollection<TagEntity>("Tags");
            _category = context.GetCollection<CategoryEntity>("Category");
            this.jwt = jwt;
        }

        // create post 

        public async Task<respons> CreatePost(PostDTO postdto,string token)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    var post = new PostEntity
                    {
                        UserID = userid,
                        Content = postdto.Content,
                        CategoryID = postdto.CategoryId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        TagID = postdto.Tags,
                        CommentID = new List<string>()
                    };

                    await _post.InsertOneAsync(post);

                    // update category's postid list
                    var update = Builders<CategoryEntity>.Update.Push(u => u.PostID, post.Id);
                    await _category.UpdateOneAsync(u => u.Id == postdto.CategoryId, update);

                    // update user's post list
                    var updateuser = Builders<UserEntity>.Update.Push(u => u.PostID, post.Id);
                    await _users.UpdateOneAsync(u => u.Id == userid, updateuser);

                    // update tag's postid list
                    if (postdto.Tags != null && postdto.Tags.Any())
                    {
                        var tagFilter = Builders<TagEntity>.Filter.In(t => t.Id, postdto.Tags);
                        var tagUpdate = Builders<TagEntity>.Update.Push(t => t.PostID, post.Id);
                        await _tag.UpdateManyAsync(tagFilter, tagUpdate);
                    }

                    return new respons(true, "Successfull created");
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // read all post
        public async Task<respons> AllPost()
        {
            try
            {
                var posts = await _post.Find(u => true).ToListAsync();
                if (posts != null)
                {
                    return new respons(true, posts);
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // read all user's post
        public async Task<respons> GetUserPosts(string userid)
        {
            try
            {
                var posts = await _post.Find(p => p.UserID == userid).ToListAsync();
                return new respons(true, posts);
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // read all post(category)
        public async Task<respons> GetPostsByCategory(string categoryId)
        {
            try
            {
                var posts = await _post.Find(p => p.CategoryID == categoryId).ToListAsync();
                return new respons(true, posts);
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // Update a post
        public async Task<respons> UpdatePost(string postId, PostDTO postdto, string token)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (!checktoken.Success)
                    return new respons(false, "Token is invalid");

                var data = checktoken.Data as Dictionary<string, string>;
                if (data == null || !data.ContainsKey("userid"))
                    return new respons(false, "User not found");

                var update = Builders<PostEntity>.Update
                    .Set(p => p.Content, postdto.Content)
                    .Set(p => p.CategoryID, postdto.CategoryId)
                    .Set(p => p.TagID, postdto.Tags)
                    .Set(p => p.UpdatedAt, DateTime.UtcNow);

                var result = await _post.UpdateOneAsync(p => p.Id == postId, update);

                return result.ModifiedCount > 0
                    ? new respons(true, "Post updated successfully")
                    : new respons(false, "Post not found or not updated");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
        // delete post
        public async Task<respons> DeletePost(string postId, string token)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (!checktoken.Success)
                    return new respons(false, "Token is invalid");

                // Get the post before deleting
                var post = await _post.Find(p => p.Id == postId).FirstOrDefaultAsync();
                if (post == null)
                    return new respons(false, "Post not found");

                // Remove postId from the Category's PostID list
                var categoryUpdate = Builders<CategoryEntity>.Update.Pull(c => c.PostID, post.Id);
                await _category.UpdateOneAsync(c => c.Id == post.CategoryID, categoryUpdate);

                // Remove postId from all associated Tags' PostID list
                var tagUpdate = Builders<TagEntity>.Update.Pull(t => t.PostID, post.Id);
                await _tag.UpdateManyAsync(Builders<TagEntity>.Filter.In(t => t.Id, post.TagID), tagUpdate);

                // Remove postId from all associated users' PostID list
                var userUpdate = Builders<UserEntity>.Update.Pull(t => t.PostID, post.Id);
                await _users.UpdateManyAsync(t => t.Id == post.UserID, userUpdate);

                // Delete the post
                var result = await _post.DeleteOneAsync(p => p.Id == postId);

                return result.DeletedCount > 0
                    ? new respons(true, "Post deleted successfully")
                    : new respons(false, "Failed to delete post");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }



        // add like count
        public async Task<respons> AddLike(string postId)
        {
            try
            {
                var update = Builders<PostEntity>.Update.Inc(p => p.Likes, 1);
                await _post.UpdateOneAsync(p => p.Id == postId, update);
                return new respons(true, "Like added");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
        // add views count
        public async Task<respons> AddView(string postId)
        {
            try
            {
                var update = Builders<PostEntity>.Update.Inc(p => p.Views, 1);
                await _post.UpdateOneAsync(p => p.Id == postId, update);
                return new respons(true, "View count increased");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
    }
}
