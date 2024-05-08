using GameStore.Domain.IRepositories;
using GameStore.Domain.UserEntities;

namespace GameStore.Domain.IUserRepositories;

public interface IRoleRepository : IRepository<RoleModel>
{
    public Task<IEnumerable<RoleModel>> GetAllRoles();

    public Task<IEnumerable<RoleModel>> GetUserRoles(Guid userId);

    public Task<RoleModel> GetDefaultRole(DefaultRoles defaultRole);
}
