namespace Api.Models;

public class CreateExpertRequest
{
    public required string Name { get; set; }
    public required string Specialization { get; set; }
    public required string Email { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
}
