using TypeRacerServer.Core.Application.Services.LeaderboardManager;

namespace TypeRacerServer.Api.Controllers;

public class Leaderboard(LeaderboardSerivce _context)
{
    public async Task LeaderboardPrinter()
    {
        await _context.Leaderboard();
    }
}