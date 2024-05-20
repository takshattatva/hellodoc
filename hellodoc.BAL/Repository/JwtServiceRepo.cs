using hellodoc.BAL.Interface;
using hellodoc.DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace hellodoc.BAL.Repository
{
    public class JwtServiceRepo : IJwtServiceRepo
    {
        private readonly IConfiguration _iconfig;

        public JwtServiceRepo(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        #region Token Generate & Validate

        //***************************************************************************************************************************************************
        /// <summary>
        /// Generate Token
        /// </summary>
        /// <param name="aspnetuser"></param>
        /// <returns></returns>
        public string GenerateJwtToken(Aspnetuser aspnetuser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfig["Jwt:key"]));
            var creadentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, aspnetuser.Email),
                new Claim(ClaimTypes.Role, aspnetuser.Aspnetuserrole.Role.Name),
                new Claim("userId", aspnetuser.Id),
            };

            var token = new JwtSecurityToken(
                _iconfig["Jwt:Issuer"],
                _iconfig["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creadentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //***************************************************************************************************************************************************
        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="jwtSecurityToken"></param>
        /// <returns></returns>
        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;

            if(token == null)
            {
                return false;
            }
            var tokenhandler = new JwtSecurityTokenHandler();
            try
            {
                tokenhandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = _iconfig["Jwt:Issuer"],
                    ValidAudience = _iconfig["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfig["Jwt:key"]))
                }, out SecurityToken validatedToken);

                jwtSecurityToken = (JwtSecurityToken)validatedToken;

                if(jwtSecurityToken != null) 
                { 
                    return jwtSecurityToken.ValidTo > DateTime.UtcNow;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************
    }
}
