using System.Security.Claims;
using CalendarScheduler.Api.Data;
using CalendarScheduler.Api.Dtos;
using CalendarScheduler.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HabitsController(AppDbContext db) : ControllerBase
{
    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Request is authenticated but has no user id claim.");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HabitResponse>>> GetHabits()
    {
        var habits = await db.Habits
            .Where(h => h.UserId == CurrentUserId && !h.IsArchived)
            .Select(h => new HabitResponse(h.Id, h.Name, h.Description, h.IsArchived, h.CreatedAtUtc))
            .ToListAsync();

        return Ok(habits);
    }

    [HttpPost]
    public async Task<ActionResult<HabitResponse>> CreateHabit(CreateHabitRequest request)
    {
        var habit = new Habit
        {
            UserId = CurrentUserId,
            Name = request.Name,
            Description = request.Description,
        };

        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var response = new HabitResponse(habit.Id, habit.Name, habit.Description, habit.IsArchived, habit.CreatedAtUtc);
        return CreatedAtAction(nameof(GetHabits), new { id = habit.Id }, response);
    }

    [HttpGet("{habitId:int}/logs")]
    public async Task<ActionResult<IEnumerable<HabitLogResponse>>> GetLogs(int habitId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
    {
        var habitExists = await db.Habits.AnyAsync(h => h.Id == habitId && h.UserId == CurrentUserId);
        if (!habitExists) return NotFound();

        var query = db.HabitLogs.Where(l => l.HabitId == habitId);
        if (from is not null) query = query.Where(l => l.Date >= from);
        if (to is not null) query = query.Where(l => l.Date <= to);

        var logs = await query
            .OrderBy(l => l.Date)
            .Select(l => new HabitLogResponse(l.Id, l.Date, l.Completed, l.Notes))
            .ToListAsync();

        return Ok(logs);
    }

    [HttpPut("{habitId:int}/logs/{date}")]
    public async Task<ActionResult<HabitLogResponse>> UpsertLog(int habitId, DateOnly date, UpsertHabitLogRequest request)
    {
        var habitExists = await db.Habits.AnyAsync(h => h.Id == habitId && h.UserId == CurrentUserId);
        if (!habitExists) return NotFound();

        var log = await db.HabitLogs.FirstOrDefaultAsync(l => l.HabitId == habitId && l.Date == date);
        if (log is null)
        {
            log = new HabitLog { HabitId = habitId, Date = date };
            db.HabitLogs.Add(log);
        }

        log.Completed = request.Completed;
        log.Notes = request.Notes;
        await db.SaveChangesAsync();

        return Ok(new HabitLogResponse(log.Id, log.Date, log.Completed, log.Notes));
    }
}
