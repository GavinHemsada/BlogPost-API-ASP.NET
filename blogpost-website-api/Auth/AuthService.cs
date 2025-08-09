using blogpost_website_api.DB;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using blogpost_website_api.Security;
using MongoDB.Driver;

namespace blogpost_website_api.Auth
{
    public class AuthService
    {
        private readonly IMongoCollection<UserEntity> _users;
        private readonly JWT jwt;
        public AuthService(MongoDBContext context,JWT jwt) {
            _users = context.GetCollection<UserEntity>("Users");
            this.jwt = jwt;
        }

        // User register logic
        public async Task<respons> Register(RegisterDTO registerdto)
        {
            try
            {
                var existingUser = await _users.Find(u => u.Email == registerdto.Email).FirstOrDefaultAsync();
                if (existingUser != null) return new respons(false, "Email Already exists");

                var user = new UserEntity
                {
                    Username = registerdto.Username,
                    Email = registerdto.Email,
                    Password = PasswordProtect.HashPassword(registerdto.Password),
                    Role = registerdto.Role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _users.InsertOneAsync(user);
                return new respons(true, "Successfull register");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
        // User login logic
        public async Task<respons> Login(LoginDTO logindto)
        {
            try
            {
                var user = await _users.Find(u => u.Email == logindto.Email).FirstOrDefaultAsync();
                if (user != null && PasswordProtect.VerifyPassword(logindto.Password, user.Password))
                {
                    var token = jwt.GenerateToken(user.Id, user.Role);
                    return new respons(true, "Login succesfull", token);
                }
                return new respons(false, "invalid Username or Password");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
    }
}
