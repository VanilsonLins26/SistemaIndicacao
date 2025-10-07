using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Services;

public interface ITokenService
{
        JwtSecurityToken GerarAccessToken(IEnumerable<Claim> claims, IConfiguration _config);
        string GerarRefreshToken();
        ClaimsPrincipal ObterDoTokenExpirado(string token, IConfiguration _config);
  

}
