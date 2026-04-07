using System.Collections.Concurrent;
using TypeRacerServer.Core.Models;
namespace TypeRacerServer.Core.State;

public class GameState
{
    public readonly ConcurrentDictionary<string, RoomState> Rooms = new();
    public readonly ConcurrentDictionary<string, PlayerSession> Sessions = new();
}