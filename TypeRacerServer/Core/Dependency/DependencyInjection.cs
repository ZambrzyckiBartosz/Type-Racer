using Microsoft.Extensions.DependencyInjection;
using TypeRacerServer.Core.Services;
using TypeRacerServer.Core.State;

namespace TypeRacerServer.Core.Dependency;

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