using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Core.Requests;

public class LoginRequest
{
    public Username Username { get; set; } = string.Empty;
    public Password Password{ get; set; } = string.Empty;
}