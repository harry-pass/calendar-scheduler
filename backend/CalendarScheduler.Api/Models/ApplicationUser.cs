using Microsoft.AspNetCore.Identity;

namespace CalendarScheduler.Api.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Habit> Habits { get; set; } = new List<Habit>();
}
