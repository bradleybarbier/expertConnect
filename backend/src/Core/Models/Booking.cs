namespace Core.Models;

public class Booking
{
    public Guid Id { get; set; }
    public Guid ExpertId { get; set; }
    public required string ClientName { get; set; }
    public required string ClientEmail { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    // Navigation property
    public Expert Expert { get; set; } = null!;
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}
