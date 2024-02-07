using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    [HttpGet("ban/durations")]
    public IActionResult GetBanDurations()
    {
        var list = new List<string> { "1 hour", "1 day", "1 week", "1 month", "permanent" };
        return Ok(list);
    }
}
