using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameStore.Domain;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.Users;
public class AuthService(IOptions<JwtSettings> jwtSettings, UserManager<PersonModel> userManager, RoleManager<RoleModel> roleManager, SignInManager<PersonModel> signInManager) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly UserManager<PersonModel> _userManager = userManager;
    private readonly RoleManager<RoleModel> _roleManager = roleManager;
    private readonly SignInManager<PersonModel> _signInManager = signInManager;

    public async Task<string> LoginUser(string username, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null ? "Bearer " + await GenerateAccessToken(user) : throw new Exception("Login failed");
        }
        else
        {
            throw new Exception("Login failed");
        }
    }

    public async Task<TokenData> GetJwtToken(PersonModel userModel)
    {
        var accessTokenStr = await GenerateAccessToken(userModel);
        var refreshTokenStr = GenerateRefreshToken(userModel);

        return new("Bearer " + accessTokenStr, "Bearer " + refreshTokenStr);
    }

    public async Task<string> GenerateAccessToken(PersonModel userModel)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
            new(ClaimTypes.Name, userModel.UserName.ToString()),
            new("IsBanned", userModel.IsBanned.ToString()),
        };

        var userRoles = await _userManager.GetRolesAsync(userModel);
        foreach (var roleName in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in roleClaims)
                {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }
            }
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var token = new JwtSecurityToken(
            null,
            null,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(PersonModel userModel)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
            new(ClaimTypes.Name, userModel.UserName.ToString()),
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(10),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
