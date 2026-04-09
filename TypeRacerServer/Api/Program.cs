using TypeRacerServer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TypeRacerServer.Api.Extensions;
using TypeRacerServer.Core.Domain.Dependency;
using TypeRacerServer.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

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
app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<GameHub>("/gamehub");

using (var scope = app.Services.CreateScope()){
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.EnsureCreated();
}


app.Run();


