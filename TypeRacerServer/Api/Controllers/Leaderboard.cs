using TypeRacerServer.Core.Requests;
using TypeRacerServer.Core.Services;

namespace TypeRacerServer.Api.Controllers;

public class Leaderboard(LeaderboardSerivce _context)
{
    public async Task LeaderboardPrinter()
    {
        await _context.Leaderboard();
    }
}