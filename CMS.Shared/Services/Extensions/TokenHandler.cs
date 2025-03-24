using CMS.Modules.Modules.ParamModel;
using CMS.Modules.Modules.ResponseModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Shared.Services.Extensions
{
    public class TokenHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenHandler> _logger;

        public string JWT_SECURITY_KEY { get; private set; }
        public int JWT_TOKEN_VALIDITY { get; private set; }

        public TokenHandler(IConfiguration configuration, ILogger<TokenHandler> logger)
        {
            _logger = logger;
            _configuration = configuration;

            // Retrieve and validate the values from appsettings.json
            JWT_SECURITY_KEY = _configuration["JWT_TOKEN:KEY"];
            if (!int.TryParse(_configuration["JWT_TOKEN:VALIDITY"], out int validity))
            {
                throw new InvalidOperationException("Invalid JWT token validity period in configuration.");
            }
            JWT_TOKEN_VALIDITY = validity;
        }

        public string GenerateToken(UserDetailsResponseModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);  // Use the JWT_SECURITY_KEY here

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, model.Email),
                    new Claim(ClaimTypes.Role, model.RoleID.ToString()),
                    new Claim(ClaimTypes.Name, model.Name),
                    new Claim(ClaimTypes.Email, model.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALIDITY),  // Use the correct validity period
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenCreate = tokenHandler.CreateToken(tokenDescription);
            var token = tokenHandler.WriteToken(tokenCreate);
            return token;
        }
    }
}
