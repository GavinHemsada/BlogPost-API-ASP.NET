using blogpost_website_api.DB;
using blogpost_website_api.DTO;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using MongoDB.Driver;

namespace blogpost_website_api.Service
{
    public class CommentService
    {
        private readonly IMongoCollection<CommentEntity> _comment;
        private readonly IMongoCollection<PostEntity> _post;
        private readonly IMongoCollection<UserEntity> _users;

        public CommentService(MongoDBContext context)
        {
            _comment = context.GetCollection<CommentEntity>("Comments");
            _post = context.GetCollection<PostEntity>("Post");
            _users = context.GetCollection<UserEntity>("Users");
        }
        // add comment
        public async Task<respons> CreateComment(CommentDTO commentDTO)
        {
            var comment = new CommentEntity
            {
                PostId = commentDTO.PostId,
                Content = commentDTO.Content,
                UserId = commentDTO.UserId, 
            };

            await _comment.InsertOneAsync(comment);

            // Add comment ID to PostEntity
            var updatePost = Builders<PostEntity>.Update.Push(p => p.CommentID, comment.Id);
            await _post.UpdateOneAsync(p => p.Id == commentDTO.PostId, updatePost);

            // Add comment ID to userEntity
            var updateUser = Builders<UserEntity>.Update.Push(p => p.CommentD, comment.Id);
            await _users.UpdateOneAsync(p => p.Id == commentDTO.PostId, updateUser);

            return new respons(true, "Comment added successfully");
        }

        // ✅ Get all comments for a post
        public async Task<respons> GetCommentsByPostId(string postId)
        {
            var comments = await _comment.Find(c => c.PostId == postId).ToListAsync();
            return comments.Count > 0
                ? new respons(true, comments)
                : new respons(false, "No comments found");
        }

        // ✅ Get single comment
        public async Task<respons> GetCommentById(string commentId)
        {
            var comment = await _comment.Find(c => c.Id == commentId).FirstOrDefaultAsync();
            return comment != null
                ? new respons(true, comment)
                : new respons(false, "Comment not found");
        }

        // ✅ Delete comment
        public async Task<respons> DeleteComment(string commentId)
        {
            // First get the comment to know which post it belongs to
            var comment = await _comment.Find(c => c.Id == commentId).FirstOrDefaultAsync();
            if (comment == null)
                return new respons(false, "Comment not found");

            // Delete comment from DB
            var result = await _comment.DeleteOneAsync(c => c.Id == commentId);

            if (result.DeletedCount > 0)
            {
                // Remove reference from PostEntity
                var updatePost = Builders<PostEntity>.Update.Pull(p => p.CommentID, commentId);
                await _post.UpdateOneAsync(p => p.Id == comment.PostId, updatePost);

                var updateUser = Builders<UserEntity>.Update.Pull(p => p.CommentD, commentId);
                await _users.UpdateOneAsync(p => p.Id == comment.PostId, updateUser);

                return new respons(true, "Comment deleted");
            }

            return new respons(false, "Failed to delete comment");
        }

    }
}
