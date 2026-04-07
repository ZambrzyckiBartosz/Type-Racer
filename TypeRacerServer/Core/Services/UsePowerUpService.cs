using TypeRacerServer.Core.State;
using TypeRacerServer.Core.Models;
namespace TypeRacerServer.Core.Services;

public class PowerUpService(GameState _gameState)
{
    public bool PowerUp(string roomCode, string TargetNick, string Power)
    {

        if (_gameState.Rooms.TryGetValue(roomCode, out var room) && room.GameStarted)
        {
            var targetId = room.Players.FirstOrDefault(x => x.Value == TargetNick).Key;
            if (targetId != null && _gameState.Sessions.TryGetValue(targetId, out var targetPlayer))
            {
                targetPlayer.DebuffsReceived++;
                if(Power == "freeze") targetPlayer.FreezeEnd = DateTime.Now.AddSeconds(3);
                return true;
            }
        }

        return false;
    }
}