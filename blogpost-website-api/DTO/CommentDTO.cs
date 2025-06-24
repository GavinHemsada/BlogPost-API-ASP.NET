namespace blogpost_website_api.DTO
{
    public class CommentDTO
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
    }
}
