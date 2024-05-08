using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Users.Repositories;

public class RolesRepository(IdentityDbContext identityDbContext) : Repository<RoleModel>(identityDbContext), IRoleRepository
{
    private readonly IdentityDbContext _identityDbContext = identityDbContext;

    public async Task<IEnumerable<RoleModel>> GetAllRoles()
    {
        return await _identityDbContext.Roles.ToListAsync();
    }

    public async Task<RoleModel> GetDefaultRole(DefaultRoles defaultRole)
    {
        return await _identityDbContext.Roles.SingleOrDefaultAsync(x => x.Name == defaultRole.ToString());
    }

    public async Task<IEnumerable<RoleModel>> GetUserRoles(Guid userId)
    {
        return await _identityDbContext.Roles.Include(x => x.Users).Where(x => x.Users.Any(y => y.Id == userId)).ToListAsync();
    }
}
