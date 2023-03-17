using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCore_API.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _config;
        public TokenServices(IConfiguration config)
        {
            _config = config;
        }
        public ResponseTokenModel BuildToken(BuildTokenModel data)
        {
            var restoken = new ResponseTokenModel();
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[] {
                new Claim(ClaimTypes.Actor, data.Username),
                new Claim(ClaimTypes.Role, data.Roles),
                new Claim(ClaimTypes.Hash, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, TimeSpan.FromMinutes(data.ExpireMinutes).ToString())
                };

                var token = new JwtSecurityToken(data.Issuer,
                    data.Issuer,
                    claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.Subtract(TimeSpan.FromMinutes(-data.ExpireMinutes)),
                    signingCredentials: credentials);

                restoken.token = new JwtSecurityTokenHandler().WriteToken(token);
                return restoken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
