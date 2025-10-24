namespace Core.Models;

public class Expert
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Specialization { get; set; }
    public required string Email { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
