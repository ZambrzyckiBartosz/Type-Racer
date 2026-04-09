using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Entities;
using TypeRacerServer.Core.Interfaces;
using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Infrastructure.Repositories;

public class RegisterRepository(AppDbContext _context) : IRegisterRepository
{
    public async Task<bool> Exists(Username username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task SaveNewUser(Username nickname, Password password)
    {
        var newUser = new User
        {
            Username = nickname,
            PasswordHash = password,
            HighScoreWpm = 0,
            GamesPlayed = 0,
            GamesWin = 0
        };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
    }
}