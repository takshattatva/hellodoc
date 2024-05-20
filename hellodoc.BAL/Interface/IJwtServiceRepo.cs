using System.IdentityModel.Tokens.Jwt;
using hellodoc.DAL.Models;

namespace hellodoc.BAL.Interface
{
    public interface IJwtServiceRepo
    {
        #region Generate Token

        public string GenerateJwtToken(Aspnetuser aspnetuser);

        #endregion


        #region Validate Token

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        #endregion
    }
}
