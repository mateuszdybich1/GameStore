using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/genres")]
[ApiController]
public class GenresController(IGenreService genreService, IGameService gameService) : ControllerBase
{
    private readonly IGenreService _genreService = genreService;
    private readonly IGameService _gameService = gameService;

    [HttpPost]
    public IActionResult AddGenre([FromBody] GenreDtoDto genreDto)
    {
        return Ok(_genreService.AddGenre(genreDto.Genre));
    }

    [HttpPut]
    public IActionResult UpdateGenre([FromBody] GenreDtoDto genreDto)
    {
        return Ok(_genreService.UpdateGenre(genreDto.Genre));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteGenre([FromRoute] Guid id)
    {
        return Ok(_genreService.DeleteGenre(id));
    }

    [HttpGet("{id}")]
    public IActionResult GetGenre([FromRoute] Guid id)
    {
        return Ok(_genreService.GetGenre(id));
    }

    [HttpGet]
    public IActionResult GetGenres()
    {
        return Ok(_genreService.GetAll());
    }

    [HttpGet("{id}/genres")]
    public IActionResult GetSubGenres([FromRoute] Guid id)
    {
        return Ok(_genreService.GetSubGenres(id));
    }

    [HttpGet("{id}/games")]
    public IActionResult GetGenreGames([FromRoute] Guid id)
    {
        return Ok(_gameService.GetGamesByGenreId(id));
    }
}
