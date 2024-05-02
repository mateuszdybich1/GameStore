using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameStore.Domain;
using GameStore.Domain.UserEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.Users;
public class AuthRepository(IOptions<JwtSettings> jwtSettings, IdentityDbContext identityDbContext) : IAuthRepository
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IdentityDbContext _identityDbContext = identityDbContext;

    public async Task<TokenData> GetJwtToken(PersonModel userModel)
    {
        var accessTokenStr = await GenerateAccessToken(userModel, _jwtSettings.SecretKey);
        var refreshTokenStr = await GenerateAccessToken(userModel, _jwtSettings.SecretKey);

        return new("Bearer " + accessTokenStr, "Bearer " + refreshTokenStr);
    }

    public async Task<string> GenerateAccessToken(PersonModel userModel, string secretKey)
    {
        var permissions = new List<Claim>();
        foreach (var role in userModel.Roles)
        {
            var roleClaims = await _identityDbContext.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();
            foreach (var claim in roleClaims)
            {
                permissions.Add(new(claim.ClaimType.ToString(), claim.ClaimValue.ToString()));
            }
        }

        var claims = new List<Claim>
        {
                new(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new(ClaimTypes.Name, userModel.Name),
        };

        claims.AddRange(permissions);

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var token = new JwtSecurityToken(
            null,
            null,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
