using Microsoft.AspNetCore.Mvc;
using TypeRacerServer.Core.Application.Requests.AccountManager;
using TypeRacerServer.Core.Application.Services.AccountManager;

namespace TypeRacerServer.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class LoginController(LoginService _user) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> LoginUser([FromBody] LoginRequest loginRequest)
    {
        await  _user.LoginHandler(loginRequest);
        return Ok();
    }
}