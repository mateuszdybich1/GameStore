using System.Security.Claims;
using GameStore.Domain;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Http;

namespace GameStore.Users;
public class HttpUserContext : IUserContext
{
    private readonly ClaimsPrincipal _user;

    public HttpUserContext(IHttpContextAccessor httpContext)
    {
        if (httpContext.HttpContext != null)
        {
            _user = httpContext.HttpContext.User;
        }
    }

    public Guid CurrentUserId
    {
        get
        {
            string id = GetIdFromClaims();
            return Guid.Parse(id);
        }
    }

    public bool IsAuthenticated => _user.Identity.IsAuthenticated;

    public string UserName => _user.Identity.Name;

    public List<Permissions> Permissions
    {
        get
        {
            var permissions = new List<Permissions>();

            var claims = _user.Claims.Where(c => c.Type == ClaimType.Permission.ToString()).ToList();

            foreach (var claim in claims)
            {
                string[] permissionsArray = claim.Value.Split(',');

                foreach (var permission in permissionsArray)
                {
                    if (Enum.TryParse(permission, out Permissions parsedPermission))
                    {
                        permissions.Add(parsedPermission);
                    }
                }
            }

            return permissions;
        }
    }

    public string GetIdFromClaims()
    {
        return _user.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
