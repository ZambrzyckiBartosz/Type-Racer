using TypeRacerServer.Core.Entities;
using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Core.Interfaces;

public interface ILoginRepository
{
     Task<User?> Login(Username Nickname);
}