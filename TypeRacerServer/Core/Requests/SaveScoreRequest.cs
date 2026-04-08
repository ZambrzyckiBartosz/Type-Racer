using TypeRacerServer.Core.ValueObjects;
namespace TypeRacerServer.Core.Requests;

public class SaveScoreRequest{
    public Username Username {get; set;} = string.Empty;
    public int WPM {get; set;}
    public bool isWinner {get; set;}
}
