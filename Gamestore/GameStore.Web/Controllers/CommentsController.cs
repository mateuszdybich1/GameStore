using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Extensions;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/comments")]
[ApiController]
public class CommentsController(IUserCheckService userCheckService, IUserService userService) : ControllerBase
{
    private readonly IUserCheckService _userCheckService = userCheckService;
    private readonly IUserService _userService = userService;

    [HttpPost("ban")]
    public async Task<IActionResult> BanUser(UserBanDto userBanDto)
    {
        try
        {
            return Ok(await _userService.BanUser(userBanDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("ban/durations")]
    public IActionResult GetBanDurations()
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Permissions.BanUser }))
        {
            var banDurations = new List<string>();
            var values = Enum.GetValues(typeof(BanDurations));

            foreach (var value in values)
            {
                if (value is BanDurations enumValue)
                {
                    banDurations.Add(EnumExtensions.GetEnumDescription(enumValue));
                }
            }

            return Ok(banDurations);
        }
        else
        {
            return Unauthorized();
        }
    }
}
