using System.Web;
using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("api/publishers")]
[ApiController]
public class PublishersController(IPublisherService publisherService, IGameService gameService) : ControllerBase
{
    private readonly IPublisherService _publisherService = publisherService;
    private readonly IGameService _gameService = gameService;

    [HttpPost]
    public async Task<IActionResult> AddPublisher([FromBody] PublisherDtoDto publisherDto)
    {
        return Ok(await _publisherService.AddPublisher(publisherDto.Publisher));
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePublisher([FromBody] PublisherDtoDto publisherDto)
    {
        return Ok(await _publisherService.UpdatePublisher(publisherDto.Publisher));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublisher([FromRoute] Guid id)
    {
        return Ok(await _publisherService.DeletePublisher(id));
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
