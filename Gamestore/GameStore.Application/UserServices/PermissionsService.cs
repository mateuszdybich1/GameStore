using GameStore.Application.IUserServices;
using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.UserServices;

public class PermissionsService(IPermissionsRepository permissionsRepository) : IPermissionsService
{
    private readonly IPermissionsRepository _permissionsRepository = permissionsRepository;

    public async Task<IEnumerable<Permissions>> GetAllPermissions()
    {
        var allPermissions = await _permissionsRepository.GetAllPermissions();

        return GetEnumPermissionsFromModel(allPermissions);
    }

    public async Task<IEnumerable<Permissions>> GetRolePermissions(Guid roleId)
    {
        var allPermissions = await _permissionsRepository.GetRolesPermissions(roleId);

        return GetEnumPermissionsFromModel(allPermissions);
    }

    private static List<Permissions> GetEnumPermissionsFromModel(IEnumerable<IdentityRoleClaim<Guid>> allPermissions)
    {
        var uniquePermissions = allPermissions.ToHashSet();

        return uniquePermissions.Select(x => (Permissions)Enum.Parse(typeof(Permissions), x.ClaimValue!)).ToList();
    }
}
