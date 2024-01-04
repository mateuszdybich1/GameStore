﻿using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        try
        {
            return Ok(_gamesService.AddGame(gameDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Please provide unique game key");
        }
    }

    [HttpGet("{key}")]
    public IActionResult GetGameByKey([FromRoute] string key)
    {
        try
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
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("find/{id}")]
    public IActionResult GetGameByKey([FromRoute] Guid id)
    {
        try
        {
            return Ok(_gamesService.GetGameById(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public IActionResult UpdateGame(GameDto gameDto)
    {
        try
        {
            return Ok(_gamesService.UpdateGame(gameDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Please provide unique game key");
        }
    }

    [HttpDelete("{key}")]
    public IActionResult DeleteGame([FromRoute] string key)
    {
        try
        {
            return Ok(_gamesService.DeleteGame(key));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetAllGames()
    {
        return Ok(_gamesService.GetGames());
    }

    [HttpGet("{key}/file")]
    public IActionResult GetGamesFile([FromRoute] string key)
    {
        try
        {
            return Ok(_gamesService.GetGameByKeyWithRelations(key));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{key}/genres")]
    public IActionResult GetGamesGenres([FromRoute] string key)
    {
        try
        {
            return Ok(_genreService.GetGamesGenres(key));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{key}/platforms")]
    public IActionResult GetGamesPlatforms([FromRoute] string key)
    {
        try
        {
            return Ok(_platformService.GetGamesPlatforms(key));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
