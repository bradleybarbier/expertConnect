using Api.Models;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class TimeSlotService
{
    private readonly ApplicationDbContext _context;
    private const int SLOT_DURATION_MINUTES = 30;
    private const int WORKING_HOUR_START = 9; // 9 AM
    private const int WORKING_HOUR_END = 17;  // 5 PM

    public TimeSlotService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(Guid expertId, DateOnly date)
    {
        // Check if expert exists and is available
        var expert = await _context.Experts
            .FirstOrDefaultAsync(e => e.Id == expertId);

        if (expert == null || !expert.IsAvailable)
        {
            return Enumerable.Empty<TimeSlot>();
        }

        // Get all bookings for the expert on the specified date
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

        var existingBookings = await _context.Bookings
            .Where(b => b.ExpertId == expertId &&
                       b.StartTime >= startOfDay &&
                       b.EndTime <= endOfDay &&
                       b.Status != BookingStatus.Cancelled)
            .ToListAsync();

        // Generate all possible time slots for the day
        var allSlots = GenerateTimeSlots(date);

        // Mark slots as unavailable if they overlap with existing bookings
        foreach (var booking in existingBookings)
        {
            allSlots = allSlots.Select(slot =>
            {
                if (DoesOverlap(slot, booking))
                {
                    slot.IsAvailable = false;
                }
                return slot;
            });
        }

        return allSlots.Where(s => s.IsAvailable);
    }

    private IEnumerable<TimeSlot> GenerateTimeSlots(DateOnly date)
    {
        var slots = new List<TimeSlot>();
        var currentTime = new DateTime(date.Year, date.Month, date.Day, WORKING_HOUR_START, 0, 0);
        var endTime = new DateTime(date.Year, date.Month, date.Day, WORKING_HOUR_END, 0, 0);

        while (currentTime < endTime)
        {
            slots.Add(new TimeSlot
            {
                StartTime = currentTime,
                EndTime = currentTime.AddMinutes(SLOT_DURATION_MINUTES),
                IsAvailable = true
            });

            currentTime = currentTime.AddMinutes(SLOT_DURATION_MINUTES);
        }

        return slots;
    }

    private bool DoesOverlap(TimeSlot slot, Booking booking)
    {
        return slot.StartTime < booking.EndTime && booking.StartTime < slot.EndTime;
    }
}
