using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/comments")]
[ApiController]
public class CommentsController(IUserCheckService userCheckService) : ControllerBase
{
    private readonly IUserCheckService _userCheckService = userCheckService;

    [HttpGet("ban/durations")]
    public IActionResult GetBanDurations()
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.BanUser }))
        {
            var list = new List<string> { "1 hour", "1 day", "1 week", "1 month", "permanent" };
            return Ok(list);
        }
        else
        {
            return Unauthorized();
        }
    }
}
