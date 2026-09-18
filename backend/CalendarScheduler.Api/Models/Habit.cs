namespace CalendarScheduler.Api.Models;

public class Habit
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; }

    public ICollection<HabitLog> Logs { get; set; } = new List<HabitLog>();
}
