using GameStore.Application.Dtos;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.IUserServices;

public interface IRolesService
{
    Task<IEnumerable<RoleModelDto>> GetAllRoles();

    Task<RoleModelDto> GetRole(Guid id);

    Task<Guid> AddRole(RoleModelDtoDto roleModelDto);

    Task<Guid> UpdateRole(RoleModelDtoDto roleModelDto);

    Task<Guid> DeleteRole(Guid id);

    Task<Guid> GetDefaultRole(DefaultRoles defaultRole);
}
