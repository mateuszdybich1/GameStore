using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.UserServices;

public class RolesService(IRoleRepository roleRepository, IPermissionsRepository permissionsRepository) : IRolesService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPermissionsRepository _permissionsRepository = permissionsRepository;

    public async Task<Guid> AddRole(RoleModelDtoDto roleModelDto)
    {
        if (await _roleRepository.GetRole(roleModelDto.Role.Name) != null)
        {
            throw new Exception("Role Exists");
        }

        RoleModel roleModel = new(roleModelDto.Role.Name);
        await _roleRepository.Add(roleModel);

        foreach (var permission in roleModelDto.Permissions)
        {
            await _permissionsRepository.AddPermission(CreateIdentityRoleClaim(roleModel.Id, permission));
        }

        return roleModel.Id;
    }

    public async Task<Guid> DeleteRole(Guid id)
    {
        var role = await _roleRepository.Get(id) ?? throw new EntityNotFoundException("Role not found");
        await _roleRepository.Delete(role);

        return role.Id;
    }

    public async Task<IEnumerable<RoleModelDto>> GetAllRoles()
    {
        var allRoles = await _roleRepository.GetAllRoles();

        return allRoles.Select(x => new RoleModelDto(x)).ToList();
    }

    public async Task<RoleModelDto> GetRole(Guid id)
    {
        var role = await _roleRepository.Get(id);
        return new(role);
    }

    public async Task<Guid> UpdateRole(RoleModelDtoDto roleModelDto)
    {
        if (roleModelDto.Role.ID == null)
        {
            throw new ArgumentNullException("RoleID mustn't be null");
        }

        var existingRole = await Get((Guid)roleModelDto.Role.ID) ?? throw new EntityNotFoundException("Role not found");
        await DeleteRole((Guid)roleModelDto.Role.ID);
        await AddRole(roleModelDto);

        return existingRole.Id;
    }

    public async Task<Guid> GetDefaultRole(DefaultRoles defaultRole)
    {
        var role = await _roleRepository.GetDefaultRole(defaultRole);

        return role.Id;
    }

    private async Task<RoleModel> Get(Guid id)
    {
        return await _roleRepository.Get(id) ?? throw new EntityNotFoundException($"Couldn't find role by ID: {id}");
    }

    private static IdentityRoleClaim<Guid> CreateIdentityRoleClaim(Guid roleModelId, Permissions permission)
    {
        return new() { RoleId = roleModelId, ClaimType = ClaimType.Permission.ToString(), ClaimValue = permission.ToString() };
    }
}
