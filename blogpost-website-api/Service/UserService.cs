using blogpost_website_api.DB;
using blogpost_website_api.DTO;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using blogpost_website_api.Security;
using MongoDB.Driver;

namespace blogpost_website_api.Service
{
    public class UserService 
    {
        private readonly IMongoCollection<UserEntity> _users;
        private readonly IMongoCollection<UserProfileEntity> _profile;
        private readonly IMongoCollection<OTPEntity> _otp;
        private readonly IMongoCollection<NotificationEntity> _notification;
        private readonly IMongoCollection<PostEntity> _post;
        private readonly IMongoCollection<CommentEntity> _comment;
        private readonly IMongoCollection<TagEntity> _tag;
        private readonly IMongoCollection<CategoryEntity> _category;
        private readonly JWT jwt;

        public UserService(MongoDBContext context, JWT jwt)
        {
            _users = context.GetCollection<UserEntity>("Users");
            _profile = context.GetCollection<UserProfileEntity>("User_Profile");
            _otp = context.GetCollection<OTPEntity>("OTP");
            _notification = context.GetCollection<NotificationEntity>("Notification");
            _post = context.GetCollection<PostEntity>("Post");
            _comment = context.GetCollection<CommentEntity>("Comment");
            _tag = context.GetCollection<TagEntity>("Tags");
            _category = context.GetCollection<CategoryEntity>("Category");
            this.jwt = jwt;
        }

        // User register logic
        public async Task<respons> Register(RegisterDTO registerdto)
        {
            try
            {
                var existingUser = await _users.Find(u => u.Email == registerdto.Email).FirstOrDefaultAsync();
                if (existingUser != null) return new respons(false,"Email Already exists");

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
                return new respons(true,"Successfull register");
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
                    var token = jwt.GenerateToken(user.Id, user.Username, user.Email, user.Role);
                    return new respons(true,"Login succesfull" ,token);
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // Create user profile 
        public async Task<respons> CreateUserProfile(UserProfileDTO userProfileDTO,string token)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    var profile = new UserProfileEntity
                    {
                        UserId = userid,
                        Location = userProfileDTO.Location,
                        Website = userProfileDTO.Website,
                        TwitterHandle = userProfileDTO.TwitterHandle,
                        LinkedInUrl = userProfileDTO.LinkedInUrl,
                        BirthDate = userProfileDTO.BirthDate,
                        Gender = userProfileDTO.Gender,
                        PhoneNumber = userProfileDTO.PhoneNumber,
                        ProfileImage = userProfileDTO.ProfileImage,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _profile.InsertOneAsync(profile);
                    return new respons(true,"Successfull Profile Created");
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // get User Profile Details
        public async Task<respons> UserProfile(string token)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    var profile = await _profile.Find(u => u.UserId == userid).FirstOrDefaultAsync();
                    return new respons(true, profile);
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // edit user details
        public async Task<respons> EditUserDetails(string token , UserDTO userDTO)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    var filter = Builders<UserEntity>.Filter.Eq(p => p.Id, userid);

                    var update = Builders<UserEntity>.Update
                        .Set(p => p.Username, userDTO.Username)
                        .Set(p => p.Email, userDTO.Email)
                        .Set(p => p.Role, userDTO.role)
                        .Set(p => p.UpdatedAt, DateTime.UtcNow);

                    var result = await _users.UpdateOneAsync(filter, update);
                    return result.ModifiedCount > 0 
                        ? new respons(true, "user details updated successfully") 
                        : new respons(false, "Cant update user details");
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex) 
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
        
        // edit user Profile details
        public async Task<respons> EditUserProfileDetails(string token, UserProfileDTO userProfileDTO)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    var filter = Builders<UserProfileEntity>.Filter.Eq(p => p.Id, userid);

                    var update = Builders<UserProfileEntity>.Update
                        .Set(p => p.Location, userProfileDTO.Location)
                        .Set(p => p.Website, userProfileDTO.Website)
                        .Set(p => p.TwitterHandle, userProfileDTO.TwitterHandle)
                        .Set(p => p.LinkedInUrl, userProfileDTO.LinkedInUrl)
                        .Set(p => p.BirthDate, userProfileDTO.BirthDate)
                        .Set(p => p.Gender, userProfileDTO.Gender)
                        .Set(p => p.PhoneNumber, userProfileDTO.PhoneNumber)
                        .Set(p => p.ProfileImage, userProfileDTO.ProfileImage)
                        .Set(p => p.LastUpdatedAt, DateTime.UtcNow);

                    var result = await _profile.UpdateOneAsync(filter, update);
                    return result.ModifiedCount > 0
                        ? new respons(true, "Profile updated successfully")
                        : new respons(false, "Cant update Profile");
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // delete User
        public async Task<respons> DeleteUser(string token)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    await _profile.DeleteOneAsync(p=>p.UserId.Equals(userid));
                    await _otp.DeleteManyAsync(o =>o.UserID.Equals(userid));
                    await _notification.DeleteManyAsync(n => n.UserID.Equals(userid));
                    await _comment.DeleteManyAsync(n => n.UserId.Equals(userid));

                    var userpost = await _post.Find(u => u.UserID.Equals(userid)).ToListAsync();

                    if (userpost.Any())
                    {
                        var postIds = userpost.Select(po => po.Id).ToList();
                        var tagIds = userpost.SelectMany(t => t.TagID).Distinct().ToList();
                        var categoryIds = userpost.SelectMany(ca => ca.CategoryID).Distinct().ToList();
                        var commentIds = userpost.SelectMany(co => co.CommentID).Distinct().ToList();

                        await _comment.DeleteManyAsync(com => postIds.Contains(com.PostId));

                        await _category.DeleteManyAsync(ca => ca.PostID.Any(p => postIds.Contains(p)));

                        var tagFilter = Builders<TagEntity>.Filter.In(tf => tf.Id, tagIds);
                        var tagUpdate = Builders<TagEntity>.Update.PullAll(tu => tu.PostID, postIds);
                        await _tag.UpdateManyAsync(tagFilter, tagUpdate);

                        await _post.DeleteManyAsync(pt => pt.UserID.Equals(userid));
                    }

                    var result = await _users.DeleteManyAsync(u => u.Id.Equals(userid));
                    return result.DeletedCount > 0
                       ? new respons(true, "User delete successfully")
                       : new respons(false, "Cant Delete user");
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

    }

}
