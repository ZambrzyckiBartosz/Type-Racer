using Microsoft.EntityFrameworkCore;

namespace TypeRacerServer;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int HighScoreWpm { get; set; } = 0;
    public int GamesPlayed { get; set; } = 0;
    public int GamesWin { get;  set; } = 0;
}

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
}
