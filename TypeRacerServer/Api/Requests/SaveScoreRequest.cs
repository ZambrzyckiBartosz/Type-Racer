namespace TypeRacerServer.Api.Requests;

public class SaveScoreRequest{
    public string Username {get; set;} = string.Empty;
    public int WPM {get; set;}
    public bool isWinner {get; set;}
}
