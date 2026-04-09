using TypeRacerServer.Core.Entities;
using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Core.Interfaces;

public interface ISaveScoreRepository
{
    Task<User?> getUser(Username username);
    Task saveScore();
}