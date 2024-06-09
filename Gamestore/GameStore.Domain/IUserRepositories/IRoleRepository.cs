using GameStore.Domain.IRepositories;
using GameStore.Domain.UserEntities;

namespace GameStore.Domain.IUserRepositories;

public interface IRoleRepository : IRepository<RoleModel>
{
    Task<IEnumerable<RoleModel>> GetAllRoles();

    Task<IEnumerable<RoleModel>> GetUserRoles(Guid userId);

    Task<RoleModel> GetDefaultRole(DefaultRoles defaultRole);

    Task<RoleModel?> GetRole(string roleName);

    Task AddUserRoles(PersonModel user, List<RoleModel> roles);

    Task UpdateUserRoles(PersonModel user, List<RoleModel> roles);
}
