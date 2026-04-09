using TypeRacerServer.Core.Interfaces;
using TypeRacerServer.Core.Requests;
namespace TypeRacerServer.Api.Services;

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