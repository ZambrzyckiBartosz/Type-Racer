using TypeRacerServer.Core.Entities;
using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Core.Interfaces;

public interface IRegisterRepository
{
    Task<bool> Exists(Username Username);
    Task SaveNewUser(Username nickname, Password password);
}