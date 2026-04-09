using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Entities;
using TypeRacerServer.Core.Interfaces;
using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Infrastructure.Repositories;
public class LoginRepository(AppDbContext _context) : ILoginRepository
{
    public async Task<User?> Login(Username Nickname)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == Nickname);
    }
}