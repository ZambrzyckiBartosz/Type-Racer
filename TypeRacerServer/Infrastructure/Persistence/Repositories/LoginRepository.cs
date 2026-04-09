using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Application.Interfaces.AccountManagerInterfaces;
using TypeRacerServer.Core.Domain.Entities;
using TypeRacerServer.Core.Domain.ValueObjects;

namespace TypeRacerServer.Infrastructure.Persistance.Repositories;
public class LoginRepository(AppDbContext _context) : ILoginRepository
{
    public async Task<User?> Login(Username Nickname)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == Nickname);
    }
}