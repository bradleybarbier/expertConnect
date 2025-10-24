using Api.Models;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class BookingService
{
    private readonly ApplicationDbContext _context;
    private readonly TimeSlotService _timeSlotService;
    private const int SLOT_DURATION_MINUTES = 30;

    public BookingService(ApplicationDbContext context, TimeSlotService timeSlotService)
    {
        _context = context;
        _timeSlotService = timeSlotService;
    }

    public async Task<(bool success, string message, Booking? booking)> CreateBookingAsync(CreateBookingRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Check if expert exists and is available
            var expert = await _context.Experts
                .FirstOrDefaultAsync(e => e.Id == request.ExpertId);

            if (expert == null)
            {
                return (false, "Expert not found", null);
            }

            if (!expert.IsAvailable)
            {
                return (false, "Expert is not available for bookings", null);
            }

            // Calculate end time
            var endTime = request.StartTime.AddMinutes(SLOT_DURATION_MINUTES);

            // Check if the slot is available
            var date = DateOnly.FromDateTime(request.StartTime.DateTime);
            var availableSlots = await _timeSlotService.GetAvailableTimeSlotsAsync(request.ExpertId, date);
            
            var isSlotAvailable = availableSlots.Any(slot => 
                slot.StartTime == request.StartTime && 
                slot.EndTime == endTime && 
                slot.IsAvailable);

            if (!isSlotAvailable)
            {
                return (false, "The requested time slot is not available", null);
            }

            // Create the booking
            var booking = new Booking
            {
                ExpertId = request.ExpertId,
                ClientName = request.ClientName,
                ClientEmail = request.ClientEmail,
                StartTime = request.StartTime,
                EndTime = endTime,
                Notes = request.Notes,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Booking created successfully", booking);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Booking?> GetBookingAsync(Guid bookingId)
    {
        return await _context.Bookings
            .Include(b => b.Expert)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }
}
