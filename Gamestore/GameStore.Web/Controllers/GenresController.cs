using GameStore.Application.Exceptions;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/genres")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly IGamesService _gamesService;

    public GenresController(IGamesService gamesService)
    {
        _gamesService = gamesService;
    }

    [HttpGet("{id}/games")]
    public IActionResult GetGenreGames([FromRoute] Guid id)
    {
        try
        {
            return Ok(_gamesService.GetGamesByGenreId(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
