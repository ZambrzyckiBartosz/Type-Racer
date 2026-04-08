using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Entities;

namespace TypeRacerServer.Infrastructure;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
}
