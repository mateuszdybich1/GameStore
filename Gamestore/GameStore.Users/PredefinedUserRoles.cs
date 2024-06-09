using System.Security.Claims;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Users;

public class PredefinedUserRoles(RoleManager<RoleModel> roleManager)
{
    private readonly RoleManager<RoleModel> _roleManager = roleManager;

    public async Task AddDefaultUserRoles()
    {
        var existingRoleNames = await _roleManager.Roles.Select(x => x.Name).ToListAsync();

        if (existingRoleNames == null)
        {
            return;
        }

        var missing = Enum.GetValues(typeof(DefaultRoles))
                 .Cast<DefaultRoles>()
                 .Where(role => !existingRoleNames.Contains(role.ToString()))
                 .ToList();

        foreach (var role in missing)
        {
            var roleTasks = new List<Task>();
            var newRole = new RoleModel(role.ToString());
            await _roleManager.CreateAsync(newRole);

            var permissions = GetDefaultPermissions(role);
            foreach (var permission in permissions)
            {
                await _roleManager.AddClaimAsync(newRole, new Claim(permission.Type, permission.Value));
            }
        }
    }

    private static List<Claim> GetDefaultPermissions(DefaultRoles defaultRole)
    {
        var permissions = new List<Claim>();

        switch (defaultRole)
        {
            case DefaultRoles.Admin:
                permissions = GetAdminPermissions();
                break;
            case DefaultRoles.Manager:
                permissions = GetMangerPermissions();
                break;
            case DefaultRoles.Moderator:
                permissions = GetModeratorPermissions();
                break;
            case DefaultRoles.User:
                permissions = GetUserPermissions();
                break;
            default:
                break;
        }

        return permissions;
    }

    private static List<Claim> GetUserPermissions()
    {
        var permissions = new List<Claim>
        {
            new(ClaimType.Permission.ToString(), Permissions.Game.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Games.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Genre.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Genres.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Platform.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Platforms.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Publisher.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Publishers.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Buy.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Basket.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.MakeOrder.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Order.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Orders.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Comments.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.AddComment.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.QuoteComment.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.ReplyComment.ToString()),
        };
        return permissions;
    }

    private static List<Claim> GetModeratorPermissions()
    {
        var permissions = new List<Claim>
        {
            new(ClaimType.Permission.ToString(), Permissions.DeleteComment.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.BanUser.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.BanComment.ToString()),
        };
        permissions.AddRange(GetUserPermissions());
        return permissions;
    }

    private static List<Claim> GetMangerPermissions()
    {
        var permissions = new List<Claim>
        {
            new(ClaimType.Permission.ToString(), Permissions.AddGame.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.DeleteGame.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdateGame.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.AddGenre.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.DeleteGenre.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdateGenre.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.AddPlatform.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.DeletePlatform.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdatePlatform.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.AddPublisher.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.DeletePublisher.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdatePublisher.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdateOrder.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.History.ToString()),
        };

        permissions.AddRange(GetModeratorPermissions());
        return permissions;
    }

    private static List<Claim> GetAdminPermissions()
    {
        var permissions = new List<Claim>
        {
            new(ClaimType.Permission.ToString(), Permissions.ViewDeletedGame.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdateDeletedGame.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Roles.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Role.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.AddRole.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.DeleteRole.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdateRole.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Users.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.User.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.AddUser.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.DeleteUser.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.UpdateUser.ToString()),
            new(ClaimType.Permission.ToString(), Permissions.Permisssions.ToString()),
        };

        permissions.AddRange(GetMangerPermissions());
        return permissions;
    }
}
