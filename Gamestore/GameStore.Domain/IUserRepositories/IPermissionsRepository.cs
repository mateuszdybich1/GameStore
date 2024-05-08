using Microsoft.AspNetCore.Identity;

namespace GameStore.Domain.IUserRepositories;

public interface IPermissionsRepository
{
    Task<IEnumerable<IdentityRoleClaim<Guid>>> GetAllPermissions();

    Task<IEnumerable<IdentityRoleClaim<Guid>>> GetRolesPermissions(Guid roleId);

    Task AddPermission(IdentityRoleClaim<Guid> permission);

    Task RemovePermission(IdentityRoleClaim<Guid> permission);
}
