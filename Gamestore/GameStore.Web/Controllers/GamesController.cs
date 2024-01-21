﻿using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/games")]
[ApiController]
public class GamesController(IGameService gamesService, IGenreService genreService, IPlatformService platformService, IPublisherService publisherService, IOrderService orderService) : ControllerBase
{
    private readonly IGameService _gamesService = gamesService;
    private readonly IGenreService _genreService = genreService;
    private readonly IPlatformService _platformService = platformService;
    private readonly IPublisherService _publisherService = publisherService;
    private readonly IOrderService _orderService = orderService;

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

    [HttpGet("{key}/publisher")]
    public IActionResult GetGamePublisher([FromRoute] string key)
    {
        return Ok(_publisherService.GetPublisherByGameKey(key));
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

    [HttpPost("{key}/buy")]
    public IActionResult AddToCart([FromRoute] string key)
    {
        Guid customerId = Guid.Parse("B30F65493C8946A79B69D91FE6577EB2");
        try
        {
            return Ok(_orderService.AddOrder(customerId, key));
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
}
