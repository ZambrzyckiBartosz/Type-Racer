using Microsoft.AspNetCore.Mvc;
using TypeRacerServer.Api.Services;
using TypeRacerServer.Core.Requests;
using TypeRacerServer.Core.Services;

namespace TypeRacerServer.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class RegisterController(RegisterService registerService)
{
    public async Task RegisterUser(RegisterRequestByMe registerRequest)
    {
        await registerService.RegisterHandler(registerRequest);
    }
}