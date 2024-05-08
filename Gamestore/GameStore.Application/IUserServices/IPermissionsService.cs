using GameStore.Domain.UserEntities;

namespace GameStore.Application.IUserServices;

public interface IPermissionsService
{
    Task<IEnumerable<Permissions>> GetAllPermissions();

    Task<IEnumerable<Permissions>> GetRolePermissions(Guid roleId);
}
