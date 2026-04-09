using Microsoft.AspNetCore.Mvc;
using TypeRacerServer.Core.Application.Requests.AccountManager;
using TypeRacerServer.Core.Application.Services.AccountManager;

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