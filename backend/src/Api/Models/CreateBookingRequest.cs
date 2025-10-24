namespace Api.Models;

public class CreateBookingRequest
{
    public Guid ExpertId { get; set; }
    public required string ClientName { get; set; }
    public required string ClientEmail { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public string? Notes { get; set; }
}
