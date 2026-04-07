using Microsoft.AspNetCore.Mvc;
using TypeRacerServer.Api.Requests;
using TypeRacerServer.Api.Services;
namespace TypeRacerServer.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class SaveScoreController(SaveScoreService _context) : ControllerBase
{
    [HttpPost]

    public async Task<ActionResult> SaveScore([FromBody] SaveScoreRequest saveScoreRequest, HttpContext httpContext)
    {
        await _context.SaveScoreHandler(saveScoreRequest, httpContext);
        return Ok();
    }
}