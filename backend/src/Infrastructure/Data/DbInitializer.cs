using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(ApplicationDbContext context, ILogger<DbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Ensure database is created and migrated
            await _context.Database.MigrateAsync();

            // Only seed if no experts exist
            if (!await _context.Experts.AnyAsync())
            {
                _logger.LogInformation("Starting to seed database...");
                await SeedDataAsync();
                _logger.LogInformation("Finished seeding database.");
            }
            else
            {
                _logger.LogInformation("Database already contains data - skipping seed.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private async Task SeedDataAsync()
    {
        var experts = new List<Expert>
        {
            new Expert
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Fixed ID for easy reference
                Name = "Dr. Jane Smith",
                Specialization = "Career Development",
                Email = "jane.smith@expertconnect.demo",
                Bio = "15+ years experience in career counseling and professional development",
                HourlyRate = 150,
                IsAvailable = true,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Expert
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Prof. Michael Chen",
                Specialization = "Technical Architecture",
                Email = "michael.chen@expertconnect.demo",
                Bio = "Senior software architect with focus on cloud-native solutions",
                HourlyRate = 200,
                IsAvailable = true,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Expert
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Sarah Johnson",
                Specialization = "Agile Coaching",
                Email = "sarah.johnson@expertconnect.demo",
                Bio = "Certified Scrum Master helping teams embrace agile methodologies",
                HourlyRate = 175,
                IsAvailable = true,
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        await _context.Experts.AddRangeAsync(experts);

        // Add some example bookings for demonstration
        var bookings = new List<Booking>
        {
            new Booking
            {
                ExpertId = experts[0].Id,
                ClientName = "Bob Wilson",
                ClientEmail = "bob.wilson@example.com",
                StartTime = DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(10), // Tomorrow at 10 AM
                EndTime = DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(10).AddMinutes(30),
                Status = BookingStatus.Confirmed,
                Notes = "Initial career consultation",
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        await _context.Bookings.AddRangeAsync(bookings);
        await _context.SaveChangesAsync();
    }
}
