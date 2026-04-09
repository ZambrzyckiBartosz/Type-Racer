using TypeRacerServer.Core.Application.Interfaces.AccountManagerInterfaces;
using TypeRacerServer.Core.Application.Requests.AccountManager;

namespace TypeRacerServer.Core.Application.Services.AccountManager;

public class RegisterService(IRegisterRepository _repository)
{
    public async Task RegisterHandler(RegisterRequestByMe request)
    {
        bool isNicknameTaken = await _repository.Exists(request.Username);
        if(isNicknameTaken){
            throw new InvalidOperationException("Username already exists");
        }

        await _repository.SaveNewUser(request.Username, BCrypt.Net.BCrypt.HashPassword(request.Password));
    }
}