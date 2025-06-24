using blogpost_website_api.Respons;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace blogpost_website_api.Security
{
    public class JWT
    {
        private readonly JwtSettings jwTSettings;

        public JWT(JwtSettings _jwTEntity)
        {
            jwTSettings = _jwTEntity;
        }
        public string GenerateToken(string userId, string username, string email , string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwTSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwTSettings.Issuer,
                audience: jwTSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwTSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Validate a token
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwTSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwTSettings.Issuer,
                ValidAudience = jwTSettings.Audience,
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
                var username = details.FindFirst(ClaimTypes.Name)?.Value;
                var role = details.FindFirst(ClaimTypes.Role)?.Value;
                var email = details.FindFirst(ClaimTypes.Email)?.Value;

                // Use dictionary
                var userdetails = new Dictionary<string, string>
                {
                    { "userid" , userId },
                    { "username" , username },
                    { "email" , email },
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
