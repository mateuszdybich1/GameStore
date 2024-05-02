using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RolesController(IRolesService rolesService, IUserCheckService userCheckService, IPermissionsService permissionsService) : ControllerBase
{
    private readonly IRolesService _rolesService = rolesService;
    private readonly IUserCheckService _userCheckService = userCheckService;
    private readonly IPermissionsService _permissionsService = permissionsService;

    [HttpPost]
    public async Task<IActionResult> AddRole(RoleModelDtoDto roleModelDtoDto)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.AddRole }))
        {
            try
            {
                return Ok(await _rolesService.AddRole(roleModelDtoDto));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRoleRole(RoleModelDtoDto roleModelDtoDto)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.UpdateRole }))
        {
            try
            {
                return Ok(await _rolesService.UpdateRole(roleModelDtoDto));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole([FromRoute] Guid id)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.DeleteRole }))
        {
            try
            {
                return Ok(await _rolesService.DeleteRole(id));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole([FromRoute] Guid id)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.Role }))
        {
            try
            {
                return Ok(await _rolesService.GetRole(id));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.Roles })
            ? Ok(await _rolesService.GetAllRoles())
            : Unauthorized();
    }

    [HttpGet("{id}/permissions")]
    public async Task<IActionResult> GetRolePermissions([FromRoute] Guid id)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.Permisssions }))
        {
            try
            {
                return Ok(await _permissionsService.GetRolePermissions(id));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetAllPermissions()
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.Permisssions })
            ? Ok(await _permissionsService.GetAllPermissions())
            : Unauthorized();
    }
}
