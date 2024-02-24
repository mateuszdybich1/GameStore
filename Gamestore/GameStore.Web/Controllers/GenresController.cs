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
    public async Task<IActionResult> AddGenre([FromBody] GenreDtoDto genreDto)
    {
        return Ok(await _genreService.AddGenre(genreDto.Genre));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGenre([FromBody] GenreDtoDto genreDto)
    {
        return Ok(await _genreService.UpdateGenre(genreDto.Genre));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre([FromRoute] Guid id)
    {
        return Ok(await _genreService.DeleteGenre(id));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenre([FromRoute] Guid id)
    {
        return Ok(await _genreService.GetGenre(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetGenres()
    {
        return Ok(await _genreService.GetAll());
    }

    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetSubGenres([FromRoute] Guid id)
    {
        return Ok(await _genreService.GetSubGenres(id));
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetGenreGames([FromRoute] Guid id)
    {
        return Ok(await _gameService.GetGamesByGenreId(id));
    }
}
