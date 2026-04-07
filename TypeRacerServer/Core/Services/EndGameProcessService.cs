using TypeRacerServer.Core.Models;
using TypeRacerServer.Core.State;

namespace TypeRacerServer.Core.Services;

public class EndGameProcessService(GameState _gameState)
{
    public string EndGameProcess(string hostCode)
    {
        if (!_gameState.Rooms.TryGetValue(hostCode, out var room))
        {
            return "None";
        }
        room.GameStarted = false;

        var playersList = room.Players.Keys
            .Select(cid => _gameState.Sessions.GetValueOrDefault(cid))
            .OfType<PlayerSession>()
            .ToList();

        var sorted = playersList
            .OrderByDescending(p => p.Progress)
            .ThenBy(p => p.FinishTime ?? DateTime.MaxValue)
            .ThenByDescending(p => p.Accuracy)
            .ThenByDescending(p => p.DebuffsReceived)
            .ThenBy(p => p.Nickname)
            .ThenBy(p => Guid.NewGuid())
            .ToList();

        string winnerNick = sorted.FirstOrDefault()?.Nickname ?? "None";
        return winnerNick;
    }
}