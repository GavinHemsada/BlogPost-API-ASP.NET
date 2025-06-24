using blogpost_website_api.DB;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using blogpost_website_api.Security;
using blogpost_website_api.Utility;
using MongoDB.Driver;

namespace blogpost_website_api.Service
{
    public class OTPService
    {
        private readonly IMongoCollection<OTPEntity> _otp;
        private readonly IMongoCollection<UserEntity> _users;
        private readonly JWT jwt;

        public OTPService(MongoDBContext context, JWT jwt)
        {
            _otp = context.GetCollection<OTPEntity>("OTP");
            _users = context.GetCollection<UserEntity>("Users");
            this.jwt = jwt;
        }

        // create otp and send it
        public async Task<respons> SendOTP(string token ,string email)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    string generateOTP = OTP.GenerateOTP(userid);
                    await EmailService.SendEmailAsync(email, "This is a OTP", generateOTP);

                    var newotp = new OTPEntity
                    {
                        UserID = userid,
                        Code = generateOTP,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _otp.InsertOneAsync(newotp);

                    var update = Builders<UserEntity>.Update.Push(u => u.OTPID, newotp.Id);
                    await _users.UpdateOneAsync(u => u.Id == userid, update);

                    return new respons(true, "OTP send successfull");

                }
                return new respons(false, "token is invalid"); 
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // check otp 
        public respons CheckOTP(string otp,string token)
        {
            try
            {
                var checktoken = jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var userid = data["userid"];
                    return OTP.ValidateOTP(userid,otp)
                        ? new respons(true, "opt is valid")
                        : new respons(false, "opt is invalid");
                }
                return new respons(false, "token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // read all otp
        public async Task<respons> AllOTP()
        {
            try
            {
                var existingUser = await _otp.Find(u => true).ToListAsync();
                return new respons(true,existingUser);
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // read one otp
        public async Task<respons> ReadUserOTP(string userid)
        {
            try
            {
                var existingUser = await _otp.Find(u => u.UserID == userid).FirstOrDefaultAsync();
                return new respons(true, "Successfull register");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
    }
}
