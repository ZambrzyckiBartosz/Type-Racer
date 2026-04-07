namespace TypeRacerServer.Core.Models;

public class JoinRoomResult
{
    public bool isSucces {get; set;}
    public bool NeedsLobbySetup {get; set;}
    public RoomState State {get; set;}
    public string? TextToLoad {get; set;}
}