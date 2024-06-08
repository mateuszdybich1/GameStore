using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Application.IUserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("api/platforms")]
[ApiController]
public class PlatformsController(IPlatformService platformService, IGameService gameService, IUserCheckService userCheckService) : ControllerBase
{
    private readonly IPlatformService _platformService = platformService;
    private readonly IGameService _gameService = gameService;
    private readonly IUserCheckService _userCheckService = userCheckService;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> AddPlatform([FromBody] PlatformDtoDto platformDto)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.AddPlatform })
            ? Ok(await _platformService.AddPlatform(platformDto.Platform))
            : Unauthorized();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> UpdatePlatform([FromBody] PlatformDtoDto platformDto)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.UpdatePlatform })
            ? Ok(await _platformService.UpdatePlatform(platformDto.Platform))
            : Unauthorized();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatform([FromRoute] Guid id)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.DeletePlatform })
            ? Ok(await _platformService.DeletePlatform(id))
            : Unauthorized();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlatform([FromRoute] Guid id)
    {
        return Ok(await _platformService.GetPlatform(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlatforms()
    {
        return Ok(await _platformService.GetAll());
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetPlatformGames([FromRoute] Guid id)
    {
        return Ok(await _gameService.GetGamesByPlatformId(id));
    }
}
