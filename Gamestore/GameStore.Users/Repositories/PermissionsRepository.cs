using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Users.Repositories;

public class PermissionsRepository(IdentityDbContext identityDbContext) : IPermissionsRepository
{
    private readonly IdentityDbContext _identityDbContext = identityDbContext;

    public async Task AddPermission(IdentityRoleClaim<Guid> permission)
    {
        _identityDbContext.RoleClaims.Add(permission);
        await _identityDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<IdentityRoleClaim<Guid>>> GetAllPermissions()
    {
        return await _identityDbContext.RoleClaims.Where(x => x.ClaimType == ClaimType.Permission.ToString()).ToListAsync();
    }

    public async Task<IEnumerable<IdentityRoleClaim<Guid>>> GetRolesPermissions(Guid roleId)
    {
        return await _identityDbContext.RoleClaims.Where(x => x.ClaimType == ClaimType.Permission.ToString() && x.RoleId == roleId).ToListAsync();
    }

    public async Task RemovePermission(IdentityRoleClaim<Guid> permission)
    {
        _identityDbContext.RoleClaims.Remove(permission);
        await _identityDbContext.SaveChangesAsync();
    }
}
