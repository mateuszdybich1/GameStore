using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Web.Controllers;
[Route("api/genres")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly IGameService _gameService;

    public GenresController(IGenreService genreService, IGameService gameService)
    {
        _genreService = genreService;
        _gameService = gameService;
    }

    [HttpPost]
    public IActionResult AddGenre(GenreDto genreDto)
    {
        try
        {
            return Ok(_genreService.AddGenre(genreDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Please provide unique genre name");
        }
    }

    [HttpPut]
    public IActionResult UpdateGenre(GenreDto genreDto)
    {
        try
        {
            return Ok(_genreService.UpdateGenre(genreDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Please provide unique genre name");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteGenre([FromRoute] Guid id)
    {
        try
        {
            return Ok(_genreService.DeleteGenre(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetGenre([FromRoute] Guid id)
    {
        try
        {
            return Ok(_genreService.GetGenre(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetGenres()
    {
        return Ok(_genreService.GetAll());
    }

    [HttpGet("{id}/genres")]
    public IActionResult GetSubGenres([FromRoute] Guid id)
    {
        try
        {
            return Ok(_genreService.GetSubGenres(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/games")]
    public IActionResult GetGenreGames([FromRoute] Guid id)
    {
        try
        {
            return Ok(_gameService.GetGamesByGenreId(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
