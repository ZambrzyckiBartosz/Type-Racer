using TypeRacerServer.Api.ValueObjects;

namespace TypeRacerServer.Api.Requests;

public class RegisterRequestByMe{
    public Username Username {get; set;} = string.Empty;
    public Password Password {get; set;} = string.Empty;
}