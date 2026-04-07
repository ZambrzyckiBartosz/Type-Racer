using TypeRacerServer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TypeRacerServer.Core.State;
using TypeRacerServer.Core.Services;
using TypeRacerServer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

string? jwtkey = builder.Configuration["JwtSettings:Key"];
if(string.IsNullOrEmpty(jwtkey)){
	throw new Exception("No jwt key");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
 {
	 options.RequireHttpsMetadata = false;
 	options.TokenValidationParameters = new TokenValidationParameters
 	{
 		ValidateIssuer = false,
 		ValidateAudience = false,
 		ValidateLifetime = true,
 		ValidateIssuerSigningKey = true,
 		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey))
 	};

    options.Events = new JwtBearerEvents
    {
	    OnMessageReceived = context =>
	    {
		    var accessToken = context.Request.Query["access_token"];
		    var path = context.HttpContext.Request.Path;

		    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/gamehub"))
		    {
			    context.Token = accessToken;
		    }
		    return Task.CompletedTask;
	    }
    };

 });

builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
	options.AddPolicy("ReactPolicy", policy => 
	{
		policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
	});
});
builder.Services.AddSingleton<GameState>();
builder.Services.AddTransient<JoinRoomService>();
builder.Services.AddTransient<StartRoomGameService>();
builder.Services.AddTransient<PerformCleanupService>();
builder.Services.AddTransient<SendProgressService>();
builder.Services.AddTransient<PowerUpService>();
builder.Services.AddTransient<RestartGameService>();
builder.Services.AddTransient<ChangeRoomSettingsService>();
builder.Services.AddTransient<EndGameProcessService>();
var app = builder.Build();



app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<GameHub>("/gamehub");

using (var scope = app.Services.CreateScope()){
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.EnsureCreated();
}


app.Run();


