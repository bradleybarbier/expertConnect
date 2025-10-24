namespace Api.Models;

public class TimeSlot
{
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public bool IsAvailable { get; set; }
}
