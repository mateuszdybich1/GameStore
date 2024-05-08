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
        RoleModel roleModel = new(roleModelDto.Role.Name);
        var roleTasks = new List<Task>() { _roleRepository.Add(roleModel) };

        foreach (var permission in roleModelDto.Permissions)
        {
            roleTasks.Add(_permissionsRepository.AddPermission(CreateIdentityRoleClaim(roleModel.Id, permission)));
        }

        await Task.WhenAll(roleTasks);

        return roleModel.Id;
    }

    public async Task<Guid> DeleteRole(Guid id)
    {
        var role = await _roleRepository.Get(id);
        var rolePermissions = await _permissionsRepository.GetRolesPermissions(role.Id);

        var roleTasks = new List<Task>() { _roleRepository.Delete(role) };
        foreach (var permission in rolePermissions)
        {
            roleTasks.Add(_permissionsRepository.RemovePermission(permission));
        }

        await Task.WhenAll(roleTasks);

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

        var existingRole = await Get((Guid)roleModelDto.Role.ID);

        existingRole.Name = roleModelDto.Role.Name;

        var existingPerimssions = await _permissionsRepository.GetRolesPermissions(existingRole.Id);
        var updateRoleTasks = new List<Task>() { _roleRepository.Update(existingRole) };

        foreach (var existingPermission in existingPerimssions)
        {
            updateRoleTasks.Add(_permissionsRepository.RemovePermission(existingPermission));
        }

        foreach (var permission in roleModelDto.Permissions)
        {
            var permissionModel = CreateIdentityRoleClaim(existingRole.Id, permission);

            updateRoleTasks.Add(_permissionsRepository.AddPermission(permissionModel));
        }

        await Task.WhenAll(updateRoleTasks);

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
