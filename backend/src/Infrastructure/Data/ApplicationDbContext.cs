using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expert> Experts => Set<Expert>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Expert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Specialization).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.HourlyRate).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.ClientName).HasMaxLength(100).IsRequired();
            entity.Property(b => b.ClientEmail).HasMaxLength(255).IsRequired();
            entity.Property(b => b.Notes).HasMaxLength(1000);

            // Configure the relationship between Booking and Expert
            entity.HasOne(b => b.Expert)
                  .WithMany(e => e.Bookings)
                  .HasForeignKey(b => b.ExpertId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
