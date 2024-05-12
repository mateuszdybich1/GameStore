using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Application.IUserServices;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GameStore.Web.Controllers;

[Route("api/games")]
[ApiController]
public class GamesController(IGameService gamesService,
    IGenreService genreService,
    IPlatformService platformService,
    IPublisherService publisherService,
    IOrderService orderService,
    ICommentService commentService,
    IUserContext userContext,
    IUserCheckService userCheckService,
    IOptions<BlobStorageConfiguration> blobStorageConfiguration,
    IMemoryCache cache) : ControllerBase
{
    private readonly IGameService _gamesService = gamesService;
    private readonly IGenreService _genreService = genreService;
    private readonly IPlatformService _platformService = platformService;
    private readonly IPublisherService _publisherService = publisherService;
    private readonly IOrderService _orderService = orderService;
    private readonly ICommentService _commentService = commentService;
    private readonly IUserContext _userContext = userContext;
    private readonly IUserCheckService _userCheckService = userCheckService;
    private readonly BlobStorageConfiguration _blobStorageConfiguration = blobStorageConfiguration.Value;
    private readonly IMemoryCache _cache = cache;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> AddGame(GameDtoDto gameDto)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.AddGame }))
        {
            if (!gameDto.Image.IsNullOrEmpty())
            {
                try
                {
                    string imageName = Guid.NewGuid().ToString();

                    var addGameTasks = new List<Task>() { UploadImage(gameDto.Image!, imageName) };

                    gameDto.Image = imageName;

                    addGameTasks.Add(_gamesService.AddGame(gameDto));

                    await Task.WhenAll(addGameTasks);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                await _gamesService.AddGame(gameDto);
                return Ok();
            }
        }
        else
        {
            return Unauthorized();
        }
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> UpdateGame(GameDtoDto gameDto)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.UpdateGame }))
        {
            if (!gameDto.Image.IsNullOrEmpty())
            {
                try
                {
                    string imageName = Guid.NewGuid().ToString();

                    var updateGameTasks = new List<Task>() { UploadImage(gameDto.Image!, imageName) };

                    gameDto.Image = imageName;

                    updateGameTasks.Add(_gamesService.UpdateGame(gameDto));

                    await Task.WhenAll(updateGameTasks);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                await _gamesService.UpdateGame(gameDto);
                return Ok();
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteGame([FromRoute] string key)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.DeleteGame }))
        {
            var imageId = await _gamesService.GetGameImageId(key);

            if (imageId != null)
            {
                try
                {
                    var deleteTasks = new List<Task>() { _gamesService.DeleteGame(key), DeleteImage((Guid)imageId!) };

                    await Task.WhenAll(deleteTasks);

                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                await _gamesService.DeleteGame(key);
                return Ok();
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGames([FromQuery] List<Guid> genres = null, [FromQuery] List<Guid> platforms = null, [FromQuery] List<Guid> publishers = null, [FromQuery] string name = null, [FromQuery] string datePublishing = null, [FromQuery] string sort = null, [FromQuery] uint page = 1, [FromQuery] string pageCount = "All", [FromQuery] int? minPrice = 0, [FromQuery] int? maxPrice = int.MaxValue)
    {
        return genres == null && platforms == null && publishers == null && name == null && datePublishing == null && sort == null && page == 1 && pageCount == "All" && minPrice == 0 && maxPrice == int.MaxValue
            ? Ok(await _gamesService.GetGames())
            : Ok(await _gamesService.GetGames(genres, platforms, publishers, name, datePublishing, sort, page, pageCount, (int)minPrice!, (int)maxPrice!));
    }

    [HttpGet("{key}/image")]
    public async Task<IActionResult> GetGamesImage([FromRoute] string key)
    {
        var imageId = await _gamesService.GetGameImageId(key);

        if (imageId != null)
        {
            try
            {
                return File(await GetImage((Guid)imageId!), "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        else
        {
            return NotFound();
        }
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("{key}/buy")]
    public async Task<IActionResult> AddToCart([FromRoute] string key)
    {
        try
        {
            return Ok(await _orderService.AddOrder(_userContext.CurrentUserId, key));
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("{key}/comments")]
    public async Task<IActionResult> AddComment([FromRoute] string key, [FromBody] CommentDtoDto commentDto)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.AddComment }))
        {
            var userName = _userContext.UserName;
            commentDto.Comment.Name = userName;
            return Ok(await _commentService.AddComment(key, commentDto));
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("{key}/comments")]
    public async Task<IActionResult> GetComments([FromRoute] string key)
    {
        return Ok(await _commentService.GetComments(key));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{key}/comments/{id}")]
    public async Task<IActionResult> Delete([FromRoute] string key, [FromRoute] Guid id)
    {
        var comment = await _commentService.GetComment(id);

        return comment.Name == _userContext.UserName || _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.DeleteComment })
            ? Ok(await _commentService.DeleteComment(key, id))
            : Unauthorized();
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

    private async Task<string> UploadImage(string image, string imageName)
    {
        byte[] imageData = Convert.FromBase64String(image.Split(",")[1]);

        CloudBlockBlob blob = GetCloudBlockBlob(imageName);
        await blob.UploadFromByteArrayAsync(imageData, 0, imageData.Length);

        _cache.Remove(imageName);
        _cache.Set(imageName, imageData, TimeSpan.FromMinutes(30));

        return blob.Name;
    }

    private async Task<byte[]> GetImage(Guid imageId)
    {
        var imageName = imageId.ToString();
        if (!_cache.TryGetValue(imageName, out byte[] imageData))
        {
            CloudBlockBlob blob = GetCloudBlockBlob(imageName);

            using MemoryStream memoryStream = new();
            await blob.DownloadToStreamAsync(memoryStream);
            imageData = memoryStream.ToArray();

            _cache.Set(imageName, imageData, TimeSpan.FromMinutes(30));
        }

        return imageData;
    }

    private async Task DeleteImage(Guid imageId)
    {
        var imageName = imageId.ToString();
        CloudBlockBlob blob = GetCloudBlockBlob(imageName);
        await blob.DeleteAsync();

        _cache.Remove(imageName);
    }

    private CloudBlockBlob GetCloudBlockBlob(string imageName)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_blobStorageConfiguration.ConnectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(_blobStorageConfiguration.ContainerName);

        return container.GetBlockBlobReference($"{imageName}.png");
    }
}
