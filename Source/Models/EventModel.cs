namespace UniPlanner.Source.Models;

internal class EventModel
{
	public string Title { get; set; } = string.Empty;
	public string? Details { get; set; } = string.Empty;
	public string? Location { get; set; } = string.Empty;
	public DateOnly Date { get; set; } = DateOnly.MaxValue;
	public bool AllDay { get; set; }
	public TimeOnly StartTime { get; set; } = TimeOnly.MinValue;
	public TimeOnly EndTime { get; set; } = TimeOnly.MaxValue;
	public int Colour { get; set; }
}

internal class EventModelGroup(DateOnly date, List<EventModel> eventList)
{
	public DateOnly Date { get; } = date;
	public List<EventModel> Events { get; } = [.. eventList.OrderBy(x => x.Date).ThenBy(x => x.AllDay).ThenBy(x => x.StartTime).ThenBy(x => x.Title)];
}