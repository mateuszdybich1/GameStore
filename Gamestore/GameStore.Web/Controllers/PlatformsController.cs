using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Web.Controllers;
[Route("api/platforms")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformService _platformService;
    private readonly IGameService _gameService;

    public PlatformsController(IPlatformService platformService, IGameService gameService)
    {
        _platformService = platformService;
        _gameService = gameService;
    }

    [HttpPost]
    public IActionResult AddPlatform(PlatformDto platformDto)
    {
        try
        {
            return Ok(_platformService.AddPlatform(platformDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Please provide unique platform type");
        }
    }

    [HttpPut]
    public IActionResult UpdatePlatform(PlatformDto platformDto)
    {
        try
        {
            return Ok(_platformService.UpdatePlatform(platformDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Please provide unique platform type");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePlatform([FromRoute] Guid id)
    {
        try
        {
            return Ok(_platformService.DeletePlatform(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetPlatform([FromRoute] Guid id)
    {
        try
        {
            return Ok(_platformService.GetPlatform(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetAllPlatforms()
    {
        return Ok(_platformService.GetAll());
    }

    [HttpGet("{id}/games")]
    public IActionResult GetPlatformGames([FromRoute] Guid id)
    {
        try
        {
            return Ok(_gameService.GetGamesByPlatformId(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
