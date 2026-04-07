using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Api.Requests;
using TypeRacerServer.Infrastructure;
namespace TypeRacerServer.Api.Services;

public class LeaderboardSerivce(AppDbContext _context)
{
    public async Task Leaderboard(SaveScoreRequest saveScoreRequest)
    {
        Console.WriteLine("Leaderboard Servicee active");
        var user = await _context.Users.OrderByDescending(u => u.GamesWin).ThenByDescending(u=> u.HighScoreWpm).Take(10).ToListAsync();

        var Top = user.Select( u => new {
            username = u.Username,
            HighScoreWpm = u.HighScoreWpm,
            GamesPlayed = u.GamesPlayed,
            GamesWin = u.GamesWin,
            Winrate = u.GamesPlayed > 0 ? Math.Round((double) u.GamesWin / u.GamesPlayed * 100, 1) : 0
        });
        Console.WriteLine("Leaderboard Servicee Deactive");
    }
}