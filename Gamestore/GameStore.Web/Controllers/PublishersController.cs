using System.Web;
using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Application.IUserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("api/publishers")]
[ApiController]
public class PublishersController(IPublisherService publisherService, IGameService gameService, IUserCheckService userCheckService) : ControllerBase
{
    private readonly IPublisherService _publisherService = publisherService;
    private readonly IGameService _gameService = gameService;
    private readonly IUserCheckService _userCheckService = userCheckService;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> AddPublisher([FromBody] PublisherDtoDto publisherDto)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.AddPublisher })
            ? Ok(await _publisherService.AddPublisher(publisherDto.Publisher))
            : Unauthorized();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> UpdatePublisher([FromBody] PublisherDtoDto publisherDto)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.UpdatePublisher })
            ? Ok(await _publisherService.UpdatePublisher(publisherDto.Publisher))
            : Unauthorized();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublisher([FromRoute] Guid id)
    {
        return _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.DeletePublisher })
            ? Ok(await _publisherService.DeletePublisher(id))
            : Unauthorized();
    }

    [HttpGet("{companyName}")]
    public async Task<IActionResult> GetPublisher([FromRoute] string companyName)
    {
        return Ok(await _publisherService.GetPublisherByCompanyName(HttpUtility.UrlDecode(companyName)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPublishers()
    {
        return Ok(await _publisherService.GetAll());
    }

    [HttpGet("{companyName}/games")]
    public async Task<IActionResult> GetPublisherGames([FromRoute] string companyName)
    {
        return Ok(await _gameService.GetGamesByPublisherName(HttpUtility.UrlDecode(companyName)));
    }
}
