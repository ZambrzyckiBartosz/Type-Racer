using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Entities;
using TypeRacerServer.Core.Interfaces;
using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Infrastructure.Repositories;

public class SaveScoreRepository(AppDbContext _context) : ISaveScoreRepository
{
    public async Task<User?> getUser(Username username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task saveScore()
    {
        await _context.SaveChangesAsync();
    }
}