using TypeRacerServer.Api.ValueObjects;
namespace TypeRacerServer.Api.Requests;

public class SaveScoreRequest{
    public Username Username {get; set;} = string.Empty;
    public int WPM {get; set;}
    public bool isWinner {get; set;}
}
