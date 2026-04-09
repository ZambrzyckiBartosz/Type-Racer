using Microsoft.AspNetCore.Mvc;
using TypeRacerServer.Core.Requests;
using TypeRacerServer.Core.Services;

namespace TypeRacerServer.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class SaveScoreController(SaveScoreService _context) : ControllerBase
{
    [HttpPost]

    public async Task<ActionResult> SaveScore([FromBody] SaveScoreRequest saveScoreRequest, HttpContext httpContext)
    {
        var usernameFromToken = httpContext.User.Identity?.Name;
        await _context.SaveScoreHandler(saveScoreRequest, usernameFromToken);
        return Ok();
    }
}