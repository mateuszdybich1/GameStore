using GameStore.Domain.UserEntities;

namespace GameStore.Application.IUserServices;

public interface IPermissionsService
{
    public Task<List<Permissions>> GetAllPermissions();

    public Task<List<Permissions>> GetRolePermissions(Guid roleId);
}
