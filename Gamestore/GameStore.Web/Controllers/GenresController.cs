using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Application.IUserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("api/genres")]
[ApiController]
public class GenresController(IGenreService genreService, IGameService gameService, IUserCheckService userCheckService) : ControllerBase
{
    private readonly IGenreService _genreService = genreService;
    private readonly IGameService _gameService = gameService;
    private readonly IUserCheckService _userCheckService = userCheckService;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> AddGenre([FromBody] GenreDtoDto genreDto)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.AddGenre })
            ? Ok(await _genreService.AddGenre(genreDto.Genre))
            : Unauthorized();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> UpdateGenre([FromBody] GenreDtoDto genreDto)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.UpdateGenre })
            ? Ok(await _genreService.UpdateGenre(genreDto.Genre))
            : Unauthorized();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre([FromRoute] Guid id)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.DeleteGenre })
            ? Ok(await _genreService.DeleteGenre(id))
            : Unauthorized();
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
