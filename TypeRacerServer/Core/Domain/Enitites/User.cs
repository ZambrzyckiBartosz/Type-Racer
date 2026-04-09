using TypeRacerServer.Core.Domain.ValueObjects;

namespace TypeRacerServer.Core.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public Username Username { get; set; } = string.Empty;
    public Password PasswordHash { get; set; } = string.Empty;
    public uint HighScoreWpm { get; set; } = 0;
    public uint GamesPlayed { get; set; } = 0;
    public uint GamesWin { get;  set; } = 0;
}