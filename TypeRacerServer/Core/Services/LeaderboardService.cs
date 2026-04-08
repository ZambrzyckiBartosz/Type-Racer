using TypeRacerServer.Core.Requests;
using TypeRacerServer.Core.Interfaces;
using TypeRacerServer.Core.Models;

namespace TypeRacerServer.Core.Services;

public class LeaderboardSerivce(ILeaderboardRepository _repository)
{
    public async Task<List<TopPlayers>> Leaderboard()
    {
        Console.WriteLine("Leaderboard Servicee active");

        var users = await _repository.GetTopUsers();

        var topPlayers = users.Select(u => new TopPlayers
        {
            Username = u.Username,
            HighScoreWpm = u.HighScoreWpm,
            GamesPlayed = u.GamesPlayed,
            GamesWin = u.GamesWin,
            Winrate = u.GamesWin > 0 ? Math.Round((double)u.GamesWin / u.GamesPlayed * 100, 1) : 0,
        });

        Console.WriteLine("Leaderboard Servicee Deactive");
        return topPlayers.ToList();
    }
}