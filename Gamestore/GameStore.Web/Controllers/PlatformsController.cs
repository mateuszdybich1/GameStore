using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/platforms")]
[ApiController]
public class PlatformsController(IPlatformService platformService, IGameService gameService) : ControllerBase
{
    private readonly IPlatformService _platformService = platformService;
    private readonly IGameService _gameService = gameService;

    [HttpPost]
    public async Task<IActionResult> AddPlatform([FromBody] PlatformDtoDto platformDto)
    {
        return Ok(await _platformService.AddPlatform(platformDto.Platform));
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePlatform([FromBody] PlatformDtoDto platformDto)
    {
        return Ok(await _platformService.UpdatePlatform(platformDto.Platform));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatform([FromRoute] Guid id)
    {
        return Ok(await _platformService.DeletePlatform(id));
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
