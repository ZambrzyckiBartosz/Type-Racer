using Microsoft.Extensions.DependencyInjection;
using TypeRacerServer.Core.Application.Services.GameManager;
using TypeRacerServer.Core.Application.Services.LeaderboardManager;
using TypeRacerServer.Core.Application.Services.PostGameManager;
using TypeRacerServer.Core.Application.Services.RoomManager;
using TypeRacerServer.Core.Domain.State;

namespace TypeRacerServer.Core.Domain.Dependency;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<GameState>();
        services.AddTransient<JoinRoomService>();
        services.AddTransient<StartRoomGameService>();
        services.AddTransient<PerformCleanupService>();
        services.AddTransient<SendProgressService>();
        services.AddTransient<PowerUpService>();
        services.AddTransient<RestartGameService>();
        services.AddTransient<ChangeRoomSettingsService>();
        services.AddTransient<EndGameProcessService>();

        return services;
    }
}