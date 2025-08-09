using blogpost_website_api.Respons;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace blogpost_website_api.Security
{
    public class JWT
    {
        private readonly IConfigurationSection jwtSettings;  

        public JWT(IConfiguration configuration)
        {
            jwtSettings =  configuration.GetSection("Jwt");
        }
        public string GenerateToken(string userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),   
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = int.TryParse(jwtSettings["ExpiryMinutes"], out int minutes) ? minutes : 60;

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Validate a token
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }

        // claim details
        public respons ClaimDetails(string token)
        {
            var details = ValidateToken(token);
            if (details != null)
            {
                var userId = details.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = details.FindFirst(ClaimTypes.Role)?.Value;

                // Use dictionary
                var userdetails = new Dictionary<string, string>
                {
                    { "userid" , userId },
                    {"Role",role }
                };

                // Use an Anonymous Object 
                //var userDetails = new {userId,username,email,role};  

                return new respons(true, userdetails);
            }

            return new respons(false);
        }
    }
}
