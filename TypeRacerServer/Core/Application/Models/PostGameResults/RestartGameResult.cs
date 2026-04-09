namespace TypeRacerServer.Core.Application.Models.PostGameResults;

public class RestartGameResult
{
    public bool isDone { get; set; } = false;
    public string roomCode { get; set; } = string.Empty;
}