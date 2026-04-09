using Microsoft.EntityFrameworkCore;
using TypeRacerServer.Core.Domain.Entities;

namespace TypeRacerServer.Infrastructure.Persistance;
public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
}
