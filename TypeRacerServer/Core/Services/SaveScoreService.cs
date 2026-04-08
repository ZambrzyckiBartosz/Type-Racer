/*using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Requests;
using TypeRacerServer.Infrastructure;
namespace TypeRacerServer.Api.Services;


public class SaveScoreService(AppDbContext _context)
{
    public async Task SaveScoreHandler(SaveScoreRequest saveScoreRequest, HttpContext httpContext)
    {
        var usernameFromToken = httpContext.User.Identity?.Name;
        if(string.IsNullOrEmpty(usernameFromToken)){
            throw new Exception("Invalid username");
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == saveScoreRequest.Username);
        if(user == null){
            throw new Exception("User not found");
        }
        user.GamesPlayed++;
        if(saveScoreRequest.isWinner){
            user.GamesWin++;
        }
        if (saveScoreRequest.WPM > user.HighScoreWpm){
            user.HighScoreWpm = saveScoreRequest.WPM;
        }

        await _context.SaveChangesAsync();
    }
}*/