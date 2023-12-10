using GameStore.Application.Exceptions;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/platforms")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IGamesService _gamesService;

    public PlatformsController(IGamesService gamesService)
    {
        _gamesService = gamesService;
    }

    [HttpGet("{id}/games")]
    public IActionResult GetPlatformGames([FromRoute] Guid id)
    {
        try
        {
            return Ok(_gamesService.GetGamesByPlatformId(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
