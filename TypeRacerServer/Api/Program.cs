using TypeRacerServer;
using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Api.Extensions;
using TypeRacerServer.Core.Application.Interfaces.AccountManagerInterfaces;
using TypeRacerServer.Core.Application.Interfaces.LeaderboardInterfaces;
using TypeRacerServer.Core.Application.Interfaces.SavingStatsInterfaces;
using TypeRacerServer.Core.Application.Services.AccountManager;
using TypeRacerServer.Core.Application.Services.GameManager;
using TypeRacerServer.Core.Application.Services.LeaderboardManager;
using TypeRacerServer.Core.Application.Services.PostGameManager;
using TypeRacerServer.Core.Application.Services.RoomManager;
using TypeRacerServer.Core.Domain.Constant;
using TypeRacerServer.Core.Domain.Dependency;
using TypeRacerServer.Core.Domain.State;
using TypeRacerServer.Infrastructure.Persistance;
using TypeRacerServer.Infrastructure.Persistance.Repositories;

var builder = WebApplication.CreateBuilder(args);

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;

string? jwtkey = builder.Configuration["JwtSettings:Key"];
if(string.IsNullOrEmpty(jwtkey)){
	throw new Exception("No jwt key");
}
builder.Services.AddAuthenticationCustom(jwtkey);
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IRegisterRepository, RegisterRepository>();
builder.Services.AddSingleton<GameState>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
builder.Services.AddScoped<ISaveScoreRepository, SaveScoreRepository>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<RegisterService>();
builder.Services.AddTransient<JoinRoomService>();
builder.Services.AddTransient<ChangeRoomSettingsService>();
builder.Services.AddTransient<RestartGameService>();
builder.Services.AddTransient<StartRoomGameService>();
builder.Services.AddTransient<PowerUpService>();
builder.Services.AddTransient<LeaderboardSerivce>();
builder.Services.AddTransient<SendProgressService>();
builder.Services.AddTransient<EndGameProcessService>();
builder.Services.AddTransient<PerformCleanupService>();
builder.Services.AddTransient<SaveScoreService>();
builder.Services.AddCors(options =>
{
	options.AddPolicy("ReactPolicy", policy => 
	{
		policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
	});
});
builder.Services.AddCoreServices();
var app = builder.Build();



app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/gamehub");

using (var scope = app.Services.CreateScope()){
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.EnsureCreated();
}


app.Run();


