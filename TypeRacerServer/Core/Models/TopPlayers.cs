using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Core.Models;

public class TopPlayers
{
    public Username Username { get; set; }
    public uint HighScoreWpm { get; set; }
    public uint GamesPlayed {get; set;}
    public uint GamesWin {get; set;}
    public double Winrate {get; set;}
}