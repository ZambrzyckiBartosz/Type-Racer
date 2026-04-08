using TypeRacerServer.Core.Entities;
namespace TypeRacerServer.Core.Interfaces;

public interface ILeaderboardRepository
{
    Task<List<User>>  GetTopUsers();
}