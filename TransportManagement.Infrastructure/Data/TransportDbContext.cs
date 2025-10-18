using Microsoft.EntityFrameworkCore;
using TransportManagement.Domain.Entities;

namespace TransportManagement.Infrastructure.Data;
public class TransportDbContext : DbContext
{

    public TransportDbContext ( DbContextOptions<TransportDbContext> options ) : base(options) { }

    protected override void OnModelCreating ( ModelBuilder modelBuilder )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Trip>()
        .HasOne(t => t.Transporter)
        .WithMany()
        .HasForeignKey(t => t.TransporterId)
        .OnDelete(DeleteBehavior.Restrict);

    }


    public DbSet<Transporter> Transporters { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Direction> Directions { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

}
