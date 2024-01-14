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
    public IActionResult AddPublisher(PublisherDto publisherDto)
    {
        return Ok(_publisherService.AddPublisher(publisherDto));
    }

    [HttpPut]
    public IActionResult UpdatePublisher(PublisherDto publisherDto)
    {
        return Ok(_publisherService.UpdatePublisher(publisherDto));
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePublisher([FromRoute] Guid id)
    {
        return Ok(_publisherService.DeletePublisher(id));
    }

    [HttpGet("{companyName}")]
    public IActionResult GetPublisher([FromRoute] string companyName)
    {
        return Ok(_publisherService.GetPublisherByCompanyName(companyName));
    }

    [HttpGet]
    public IActionResult GetAllPublishers()
    {
        return Ok(_publisherService.GetAll());
    }

    [HttpGet("{companyName}/games")]
    public IActionResult GetPublisherGames([FromRoute] string companyName)
    {
        return Ok(_gameService.GetGamesByPublisherName(companyName));
    }
}
