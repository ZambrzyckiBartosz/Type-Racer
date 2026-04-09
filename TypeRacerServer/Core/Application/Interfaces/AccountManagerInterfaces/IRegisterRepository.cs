using TypeRacerServer.Core.Domain.ValueObjects;

namespace TypeRacerServer.Core.Application.Interfaces.AccountManagerInterfaces;

public interface IRegisterRepository
{
    Task<bool> Exists(Username Username);
    Task SaveNewUser(Username nickname, Password password);
}