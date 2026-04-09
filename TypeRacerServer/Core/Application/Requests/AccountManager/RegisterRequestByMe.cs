using TypeRacerServer.Core.Domain.ValueObjects;

namespace TypeRacerServer.Core.Application.Requests.AccountManager;

public class RegisterRequestByMe{
    public Username Username {get; set;} = string.Empty;
    public Password Password {get; set;} = string.Empty;
}