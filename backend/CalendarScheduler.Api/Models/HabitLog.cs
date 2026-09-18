namespace CalendarScheduler.Api.Models;

public class HabitLog
{
    public int Id { get; set; }
    public int HabitId { get; set; }
    public Habit? Habit { get; set; }

    // Date-only: one log entry per habit per calendar day.
    public DateOnly Date { get; set; }
    public bool Completed { get; set; }
    public string? Notes { get; set; }
}
