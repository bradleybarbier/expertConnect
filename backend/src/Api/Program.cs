using Api.Models;
using Api.Services;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=expertConnect.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Register services
builder.Services.AddScoped<TimeSlotService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Skip HTTPS redirection in development
}
else 
{
    app.UseHttpsRedirection();
}

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitializeAsync();
}

// Expert endpoints
app.MapPost("/api/experts", async (CreateExpertRequest request, ApplicationDbContext db) =>
{
    var expert = new Expert
    {
        Name = request.Name,
        Specialization = request.Specialization,
        Email = request.Email,
        Bio = request.Bio,
        HourlyRate = request.HourlyRate,
        IsAvailable = true,
        CreatedAt = DateTimeOffset.UtcNow
    };

    db.Experts.Add(expert);
    await db.SaveChangesAsync();

    return Results.Created($"/api/experts/{expert.Id}", expert);
})
.WithName("CreateExpert")
.WithOpenApi();

app.MapGet("/api/experts", async (ApplicationDbContext db) =>
    await db.Experts.ToListAsync())
.WithName("GetExperts")
.WithOpenApi();

app.MapGet("/api/experts/{id}", async (Guid id, ApplicationDbContext db) =>
{
    var expert = await db.Experts.FindAsync(id);
    return expert is null ? Results.NotFound() : Results.Ok(expert);
})
.WithName("GetExpert")
.WithOpenApi();

// Time slot availability endpoint
app.MapGet("/api/experts/{expertId}/available-slots", async (Guid expertId, DateOnly date, TimeSlotService timeSlotService) =>
{
    var slots = await timeSlotService.GetAvailableTimeSlotsAsync(expertId, date);
    return Results.Ok(slots);
})
.WithName("GetAvailableTimeSlots")
.WithOpenApi();

// Booking endpoints
app.MapPost("/api/bookings", async (CreateBookingRequest request, BookingService bookingService) =>
{
    var (success, message, booking) = await bookingService.CreateBookingAsync(request);
    
    if (!success)
    {
        return Results.BadRequest(new { message });
    }

    return Results.Created($"/api/bookings/{booking!.Id}", booking);
})
.WithName("CreateBooking")
.WithOpenApi();

app.MapGet("/api/bookings/{id}", async (Guid id, BookingService bookingService) =>
{
    var booking = await bookingService.GetBookingAsync(id);
    return booking is null ? Results.NotFound() : Results.Ok(booking);
})
.WithName("GetBooking")
.WithOpenApi();

app.Run();