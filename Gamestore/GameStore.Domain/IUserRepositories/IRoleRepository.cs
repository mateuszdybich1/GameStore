using GameStore.Domain.IRepositories;
using GameStore.Domain.UserEntities;

namespace GameStore.Domain.IUserRepositories;

public interface IRoleRepository : IRepository<RoleModel>
{
    public Task<List<RoleModel>> GetAllRoles();

    public Task<List<RoleModel>> GetUserRoles(Guid userId);

    public Task<RoleModel> GetDefaultRole(DefaultRoles defaultRole);
}
