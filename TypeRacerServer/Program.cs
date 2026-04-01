using TypeRacerServer;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddCors(options =>
{
	options.AddPolicy("ReactPolicy", policy => 
	{
		policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
	});
});

var app = builder.Build();


//app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GameHub>("/gamehub");

app.MapPost("/api/register", async (AppDbContext db, RegisterRequest request) => {
	bool userExists = await db.Users.AnyAsync(u => u.Username == request.Username);
	if(userExists){
		return Results.BadRequest("User exists");
	}	
	var newUser = new User {
		Username = request.Username,
		PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
		GamesPlayed = 0,
		HighScoreWpm = 0,
		GamesWin = 0
	};
	db.Users.Add(newUser);
	await db.SaveChangesAsync();

	return Results.Ok("Registered");
});

app.MapPost("/api/login", async (AppDbContext db, LoginRequest request) => {
	var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

	if(user == null){
		return Results.BadRequest("Wrong login");
	}	
	bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password,user.PasswordHash);
	if(!isPasswordValid){
		return Results.BadRequest("Wrong password");
	}

	var TokenHandler = new JwtSecurityTokenHandler();
	var key = Encoding.UTF8.GetBytes(jwtkey);
	var tokenDescriptor = new SecurityTokenDescriptor{
		Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Username)}),
		Expires = DateTime.UtcNow.AddDays(7),
		SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
	};

	var token = TokenHandler.CreateToken(tokenDescriptor);
	var tokenString = TokenHandler.WriteToken(token);
	return Results.Ok(new {
		Message = "Logged in",
		Username = user.Username,
		Token = tokenString
	});
});

app.MapPost("/api/save-score", async (AppDbContext db, SaveScoreRequest request, HttpContext httpContext) => {
	var usernameFromToken = httpContext.User.Identity?.Name;
	if(string.IsNullOrEmpty(usernameFromToken)){
		return Results.Unauthorized();
	}
	var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
	if(user == null){
		return Results.BadRequest("Error");
	}	
	user.GamesPlayed++;
	if(request.isWinner){
		user.GamesWin++;
	}
	if (request.WPM > user.HighScoreWpm){
		user.HighScoreWpm = request.WPM;
	}

	await db.SaveChangesAsync();
	return Results.Ok("Score saved");
});

app.MapGet("/api/leaderboard", async (AppDbContext db) => {
	var user = await db.Users.OrderByDescending(u => u.GamesWin).ThenByDescending(u=> u.HighScoreWpm).Take(10).ToListAsync();

	var Top = user.Select( u => new {
		username = u.Username,
		HighScoreWpm = u.HighScoreWpm,
		GamesPlayed = u.GamesPlayed,
		GamesWin = u.GamesWin,
		Winrate = u.GamesPlayed > 0 ? Math.Round((double) u.GamesWin / u.GamesPlayed * 100, 1) : 0	
	});

	return Results.Ok(Top);
});

using (var scope = app.Services.CreateScope()){
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.EnsureCreated();
}


app.Run();


public class RegisterRequest{
	public string Username {get; set;} = string.Empty;
	public string Password {get; set;} = string.Empty;
}

public class LoginRequest{
	public string Username { get; set; } = string.Empty;
	public string Password{ get; set; } = string.Empty;
}

public class SaveScoreRequest{
	public string Username {get; set;} = string.Empty;
	public int WPM {get; set;}
	public bool isWinner {get; set;}
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
