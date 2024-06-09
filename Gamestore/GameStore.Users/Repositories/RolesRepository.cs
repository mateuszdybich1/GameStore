using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Users.Repositories;

public class RolesRepository(UserManager<PersonModel> userManager, RoleManager<RoleModel> roleManager) : IRoleRepository
{
    private readonly UserManager<PersonModel> _userManager = userManager;
    private readonly RoleManager<RoleModel> _roleManager = roleManager;

    public async Task Add(RoleModel entity)
    {
        await _roleManager.CreateAsync(entity);
    }

    public async Task Delete(RoleModel entity)
    {
        await _roleManager.DeleteAsync(entity);
    }

    public async Task<RoleModel> Get(Guid id)
    {
        return await _roleManager.FindByIdAsync(id.ToString());
    }

    public async Task<IEnumerable<RoleModel>> GetAllRoles()
    {
        return await _roleManager.Roles.ToListAsync();
    }

    public async Task<RoleModel> GetDefaultRole(DefaultRoles defaultRole)
    {
        return await _roleManager.FindByNameAsync(defaultRole.ToString());
    }

    public async Task<IEnumerable<RoleModel>> GetUserRoles(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = new List<RoleModel>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }
        else
        {
            return Enumerable.Empty<RoleModel>();
        }
    }

    public async Task Update(RoleModel entity)
    {
        await _roleManager.UpdateAsync(entity);
    }

    public async Task AddUserRoles(PersonModel user, List<RoleModel> roles)
    {
        await _userManager.AddToRolesAsync(user, roles.Select(x => x.Name.ToString()));
    }

    public async Task UpdateUserRoles(PersonModel user, List<RoleModel> roles)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var updateUserRolesTasks = new List<Task>
        {
            _userManager.RemoveFromRolesAsync(user, userRoles),
            _userManager.AddToRolesAsync(user, roles.Select(x => x.Name.ToString())),
        };

        await Task.WhenAll(updateUserRolesTasks);
    }

    public async Task<RoleModel?> GetRole(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }
}
