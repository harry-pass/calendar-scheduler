namespace CalendarScheduler.Api.Dtos;

public record HabitResponse(int Id, string Name, string? Description, bool IsArchived, DateTime CreatedAtUtc);

public record CreateHabitRequest(string Name, string? Description);

public record UpsertHabitLogRequest(bool Completed, string? Notes);

public record HabitLogResponse(int Id, DateOnly Date, bool Completed, string? Notes);
