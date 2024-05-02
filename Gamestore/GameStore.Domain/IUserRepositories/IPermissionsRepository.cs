using Microsoft.AspNetCore.Identity;

namespace GameStore.Domain.IUserRepositories;

public interface IPermissionsRepository
{
    public Task<List<IdentityRoleClaim<Guid>>> GetAllPermissions();

    public Task<List<IdentityRoleClaim<Guid>>> GetRolesPermissions(Guid roleId);

    public Task AddPermission(IdentityRoleClaim<Guid> permission);

    public Task RemovePermission(IdentityRoleClaim<Guid> permission);
}
