using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.Models;

internal class EventModel : ViewModelBase
{
	private string title = string.Empty;
	private string? details = null;
	private string? location = null;
	private DateOnly date = DateOnly.MaxValue;
	private bool allDay = false;
	private TimeOnly startTime = TimeOnly.MinValue;
	private TimeOnly endTime = TimeOnly.MaxValue;
	private int colour = 0;

	public string Title
	{
		get => title;
		set => SetValue(ref title, value);
	}
	public string? Details
	{
		get => details;
		set => SetValue(ref details, value);
	}
	public string? Location
	{
		get => location;
		set => SetValue(ref location, value);
	}
	public DateOnly Date
	{
		get => date;
		set => SetValue(ref date, value);
	}
	public bool AllDay
	{
		get => allDay;
		set => SetValue(ref allDay, value);
	}
	public TimeOnly StartTime
	{
		get => startTime;
		set => SetValue(ref startTime, value);
	}
	public TimeOnly EndTime
	{
		get => endTime;
		set => SetValue(ref endTime, value);
	}
	public int Colour
	{
		get => colour;
		set => SetValue(ref colour, value);
	}
}

internal class EventModelGroup(DateOnly date, List<EventModel> events) : ViewModelBase
{
	private DateOnly date = date;
	private List<EventModel> events = [.. events.OrderBy(x => x.Date).ThenBy(x => x.AllDay).ThenBy(x => x.StartTime).ThenBy(x => x.Title)];

	public DateOnly Date
	{
		get => date;
		set => SetValue(ref date, value);
	}
	public List<EventModel> Events
	{
		get => events;
		set => SetValue(ref events, value);
	}
}