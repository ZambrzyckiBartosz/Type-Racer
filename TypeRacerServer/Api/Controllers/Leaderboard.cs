using TypeRacerServer.Api.Requests;
using TypeRacerServer.Api.Services;

namespace TypeRacerServer.Api.Controllers;

public class Leaderboard(LeaderboardSerivce _context)
{
    public async Task LeaderboardPrinter(SaveScoreRequest saveScoreRequest)
    {
        await _context.Leaderboard(saveScoreRequest);
    }
}