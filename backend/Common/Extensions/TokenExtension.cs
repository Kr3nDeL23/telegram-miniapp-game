using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using Presentation.Common.Domain.Configurations;

namespace Presentation.Common.Extensions;
public class TokenExtension
{
    public static (string, DateTime) GenerateAccessToken(TokenConfiguration configuration, IEnumerable<Claim> claims)
    {
        var key = Encoding.UTF8.GetBytes(configuration.Secret);

        var signinCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var expire = DateTime.UtcNow.AddMinutes(configuration.AccessTokenExpiration);

        var securityToken = new JwtSecurityToken(
            issuer: configuration.Issuer,
            audience: configuration.Audience,
            claims: claims,
            expires: expire,
            signingCredentials: signinCredentials
        );
        return (new JwtSecurityTokenHandler().WriteToken(securityToken), expire);
    }
}
