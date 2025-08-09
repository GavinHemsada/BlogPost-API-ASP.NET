namespace blogpost_website_api.DTO
{
    public class PostDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string CategoryId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
