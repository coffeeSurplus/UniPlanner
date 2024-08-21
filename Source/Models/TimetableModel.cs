using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.Models;

internal class TimetableModel : ViewModelBase
{
	private string title = string.Empty;
	private string? details = null;
	private string? subject = null;
	private string? location = null;
	private int day = 0;
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
	public string? Subject
	{
		get => subject;
		set => SetValue(ref subject, value);
	}
	public string? Location
	{
		get => location;
		set => SetValue(ref location, value);
	}
	public int Day
	{
		get => day;
		set => SetValue(ref day, value);
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