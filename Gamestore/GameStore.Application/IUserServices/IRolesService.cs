using GameStore.Application.Dtos;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.IUserServices;

public interface IRolesService
{
    public Task<List<RoleModelDto>> GetAllRoles();

    public Task<RoleModelDto> GetRole(Guid id);

    public Task<Guid> AddRole(RoleModelDtoDto roleModelDto);

    public Task<Guid> UpdateRole(RoleModelDtoDto roleModelDto);

    public Task<Guid> DeleteRole(Guid id);

    public Task<Guid> GetDefaultRole(DefaultRoles defaultRole);
}
