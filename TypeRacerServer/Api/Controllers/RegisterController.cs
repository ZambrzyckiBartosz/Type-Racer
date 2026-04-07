using Microsoft.AspNetCore.Mvc;
using TypeRacerServer.Api.Requests;
using TypeRacerServer.Api.Services;

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