namespace TypeRacerServer.Core.Models;

public class PerformCleanupResult
{
    public bool isRemoved {get; set;}
    public bool isRommEmpty {get; set;}
    public RoomState? roomStateInfo {get; set;}
    public string roomCode {get; set;} = string.Empty;
}