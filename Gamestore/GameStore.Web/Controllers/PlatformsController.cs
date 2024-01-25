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
    public IActionResult AddPlatform([FromBody] PlatformDtoDto platformDto)
    {
        return Ok(_platformService.AddPlatform(platformDto.Platform));
    }

    [HttpPut]
    public IActionResult UpdatePlatform([FromBody] PlatformDtoDto platformDto)
    {
        return Ok(_platformService.UpdatePlatform(platformDto.Platform));
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePlatform([FromRoute] Guid id)
    {
        return Ok(_platformService.DeletePlatform(id));
    }

    [HttpGet("{id}")]
    public IActionResult GetPlatform([FromRoute] Guid id)
    {
        return Ok(_platformService.GetPlatform(id));
    }

    [HttpGet]
    public IActionResult GetAllPlatforms()
    {
        return Ok(_platformService.GetAll());
    }

    [HttpGet("{id}/games")]
    public IActionResult GetPlatformGames([FromRoute] Guid id)
    {
        return Ok(_gameService.GetGamesByPlatformId(id));
    }
}
