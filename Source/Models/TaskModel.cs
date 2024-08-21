using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.Models;

internal class TaskModel() : ViewModelBase
{
	private bool completed = false;
	private string title = string.Empty;
	private string? details = null;
	private string? subject = null;
	private DateOnly date = DateOnly.MaxValue;
	private int priority = 4;
	private List<SubtaskModel> subtasks = [];

	public bool Completed
	{
		get => completed;
		set => SetValue(ref completed, value);
	}
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
	public DateOnly Date
	{
		get => date;
		set => SetValue(ref date, value);
	}
	public int Priority
	{
		get => priority;
		set => SetValue(ref priority, value);
	}
	public List<SubtaskModel> Subtasks
	{
		get => subtasks;
		set => SetValue(ref subtasks, value);
	}

	public TaskModel(TaskModel taskModel) : this()
	{
		completed = taskModel.Completed;
		title = taskModel.Title;
		details = taskModel.Details;
		subject = taskModel.Subject;
		date = taskModel.Date;
		priority = taskModel.Priority;
		subtasks = taskModel.Subtasks.Select(x => new SubtaskModel(x)).ToList();
	}
}

internal class SubtaskModel() : ViewModelBase
{
	private bool completed = false;
	private string title = string.Empty;

	public bool Completed
	{
		get => completed;
		set => SetValue(ref completed, value);
	}
	public string Title
	{
		get => title;
		set => SetValue(ref title, value);
	}

	public SubtaskModel(SubtaskModel subtaskModel) : this()
	{
		completed = subtaskModel.Completed;
		title = subtaskModel.Title;
	}
}