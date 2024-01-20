using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/games")]
[ApiController]
public class GamesController(IGameService gamesService, IGenreService genreService, IPlatformService platformService) : ControllerBase
{
    private readonly IGameService _gamesService = gamesService;
    private readonly IGenreService _genreService = genreService;
    private readonly IPlatformService _platformService = platformService;

    [HttpPost]
    public IActionResult AddGame(GameDto gameDto)
    {
        return Ok(_gamesService.AddGame(gameDto));
    }

    [HttpGet("{key}")]
    public IActionResult GetGameByKey([FromRoute] string key)
    {
        GameDto gamedto = _gamesService.GetGameByKey(key);

        var responseDto = new
        {
            gamedto.GameId,
            gamedto.Name,
            gamedto.Description,
            gamedto.Key,
        };

        return Ok(responseDto);
    }

    [HttpGet("find/{id}")]
    public IActionResult GetGameByKey([FromRoute] Guid id)
    {
        return Ok(_gamesService.GetGameById(id));
    }

    [HttpPut]
    public IActionResult UpdateGame(GameDto gameDto)
    {
        return Ok(_gamesService.UpdateGame(gameDto));
    }

    [HttpDelete("{key}")]
    public IActionResult DeleteGame([FromRoute] string key)
    {
        return Ok(_gamesService.DeleteGame(key));
    }

    [HttpGet]
    public IActionResult GetAllGames()
    {
        return Ok(_gamesService.GetGames());
    }

    [HttpGet("{key}/file")]
    public IActionResult GetGamesFile([FromRoute] string key)
    {
        return Ok(_gamesService.GetGameByKeyWithRelations(key));
    }

    [HttpGet("{key}/genres")]
    public IActionResult GetGamesGenres([FromRoute] string key)
    {
        return Ok(_genreService.GetGamesGenres(key));
    }

    [HttpGet("{key}/platforms")]
    public IActionResult GetGamesPlatforms([FromRoute] string key)
    {
        return Ok(_platformService.GetGamesPlatforms(key));
    }
}
