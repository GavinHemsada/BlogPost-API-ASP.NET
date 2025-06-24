namespace blogpost_website_api.Security
{
    public class PasswordProtect
    {
        // Encrypt (hash) the password
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Compare the plain password with the hashed password
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
