using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/games")]
[ApiController]
public class GamesController(IGameService gamesService, IGenreService genreService, IPlatformService platformService, IPublisherService publisherService, IOrderService orderService, ICommentService commentService) : ControllerBase
{
    private readonly IGameService _gamesService = gamesService;
    private readonly IGenreService _genreService = genreService;
    private readonly IPlatformService _platformService = platformService;
    private readonly IPublisherService _publisherService = publisherService;
    private readonly IOrderService _orderService = orderService;
    private readonly ICommentService _commentService = commentService;

    private readonly Guid _customerId = Guid.Parse("3fa85f6457174562b3fc2c963f66afa6");

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    [HttpPost]
    public async Task<IActionResult> AddGame(GameDtoDto gameDto)
    {
        return Ok(await _gamesService.AddGame(gameDto));
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetGameByKey([FromRoute] string key)
    {
        return Ok(await _gamesService.GetGameByKey(key));
    }

    [HttpGet("find/{id}")]
    public async Task<IActionResult> GetGameById([FromRoute] Guid id)
    {
        return Ok(await _gamesService.GetGameById(id));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGame(GameDtoDto gameDto)
    {
        return Ok(await _gamesService.UpdateGame(gameDto));
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteGame([FromRoute] string key)
    {
        return Ok(await _gamesService.DeleteGame(key));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGames([FromQuery] List<Guid> genres = null, [FromQuery] List<Guid> platforms = null, [FromQuery] List<Guid> publishers = null, [FromQuery] string name = null, [FromQuery] string datePublishing = null, [FromQuery] string sort = null, [FromQuery] uint page = 1, [FromQuery] string pageCount = "All", [FromQuery] int? minPrice = 0, [FromQuery] int? maxPrice = int.MaxValue)
    {
        return genres == null && platforms == null && publishers == null && name == null && datePublishing == null && sort == null && page == 1 && pageCount == "All" && minPrice == 0 && maxPrice == int.MaxValue
            ? Ok(await _gamesService.GetGames())
            : Ok(await _gamesService.GetGames(genres, platforms, publishers, name, datePublishing, sort, page, pageCount, (int)minPrice!, (int)maxPrice!));
    }

    [HttpGet("{key}/file")]
    public async Task<IActionResult> GetGamesFile([FromRoute] string key)
    {
        string fileName = $"{key}";

        var game = await _gamesService.GetGameByKeyWithRelations(key);

        string serializedGame = JsonSerializer.Serialize(game, _jsonSerializerOptions);

        byte[] fileContents = Encoding.UTF8.GetBytes(serializedGame);

        return File(fileContents, "text/plain", fileName);
    }

    [HttpGet("{key}/publisher")]
    public async Task<IActionResult> GetGamePublisher([FromRoute] string key)
    {
        return Ok(await _publisherService.GetPublisherByGameKey(key));
    }

    [HttpGet("{key}/genres")]
    public async Task<IActionResult> GetGamesGenres([FromRoute] string key)
    {
        return Ok(await _genreService.GetGamesGenres(key));
    }

    [HttpGet("{key}/platforms")]
    public async Task<IActionResult> GetGamesPlatforms([FromRoute] string key)
    {
        return Ok(await _platformService.GetGamesPlatforms(key));
    }

    [HttpPost("{key}/buy")]
    public async Task<IActionResult> AddToCart([FromRoute] string key)
    {
        try
        {
            return Ok(await _orderService.AddOrder(_customerId, key));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{key}/comments")]
    public async Task<IActionResult> AddComment([FromRoute] string key, [FromBody] CommentDtoDto commentDto)
    {
        return Ok(await _commentService.AddComment(key, commentDto));
    }

    [HttpGet("{key}/comments")]
    public async Task<IActionResult> GetComments([FromRoute] string key)
    {
        return Ok(await _commentService.GetComments(key));
    }

    [HttpDelete("{key}/comments/{id}")]
    public async Task<IActionResult> Delete([FromRoute] string key, [FromRoute] Guid id)
    {
        return Ok(await _commentService.DeleteComment(key, id));
    }

    [HttpGet("pagination-options")]
    public IActionResult GetNumberOfGamesForPage()
    {
        var numberOfGames = new List<string>();
        var values = Enum.GetValues(typeof(NumberOfGamesOnPageFilteringMode));

        foreach (var value in values)
        {
            if (value is NumberOfGamesOnPageFilteringMode enumValue)
            {
                numberOfGames.Add(EnumExtensions.GetEnumDescription(enumValue));
            }
        }

        return Ok(numberOfGames);
    }

    [HttpGet("sorting-options")]
    public IActionResult GetSortingOptions()
    {
        var sortingOptions = new List<string>();
        var values = Enum.GetValues(typeof(GameSortingMode));

        foreach (var value in values)
        {
            if (value is GameSortingMode enumValue)
            {
                sortingOptions.Add(EnumExtensions.GetEnumDescription(enumValue));
            }
        }

        return Ok(sortingOptions);
    }

    [HttpGet("publish-date-options")]
    public IActionResult GetPublishDateOptions()
    {
        var publishDateList = new List<string>();
        var values = Enum.GetValues(typeof(PublishDateFilteringMode));

        foreach (var value in values)
        {
            if (value is PublishDateFilteringMode enumValue)
            {
                publishDateList.Add(EnumExtensions.GetEnumDescription(enumValue));
            }
        }

        return Ok(publishDateList);
    }
}
