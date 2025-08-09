namespace blogpost_website_api.DTO
{
    public class UserProfileDTO
    {
        public string UserId { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public string TwitterHandle { get; set; }
        public string LinkedInUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string LastUpdatedAt { get; set; }
    }
}
