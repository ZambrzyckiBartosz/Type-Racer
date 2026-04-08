/*using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Requests;
using TypeRacerServer.Infrastructure;
namespace TypeRacerServer.Api.Services;


public class RegisterService(AppDbContext _context)
{
    public async Task RegisterHandler(RegisterRequestByMe request)
    {
        bool userExists = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if(userExists){
            throw new InvalidOperationException("Username already exists");
        }
        var newUser = new User {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            GamesPlayed = 0,
            HighScoreWpm = 0,
            GamesWin = 0
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
    }
}*/