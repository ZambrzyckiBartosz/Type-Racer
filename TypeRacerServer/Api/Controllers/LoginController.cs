using Microsoft.AspNetCore.Mvc;
using TypeRacerServer.Api.Requests;
using TypeRacerServer.Api.Services;

namespace TypeRacerServer.Api.Controllers;

[ApiController]
[Route("[api/controller]")]

public class LoginController(LoginService _user) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> LoginUser([FromBody] LoginRequest loginRequest)
    {
        await  _user.LoginHandler(loginRequest);
        return Ok();
    }
}