namespace UniPlanner.Source.Models;

internal class TimetableModel
{
	public string Title { get; set; } = string.Empty;
	public string? Details { get; set; }
	public string? Subject { get; set; }
	public string? Location { get; set; }
	public int Day { get; set; }
	public TimeOnly StartTime { get; set; } = TimeOnly.MinValue;
	public TimeOnly EndTime { get; set; } = TimeOnly.MaxValue;
	public int Colour { get; set; }
}