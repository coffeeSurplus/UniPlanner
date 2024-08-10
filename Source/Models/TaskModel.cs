namespace UniPlanner.Source.Models;

internal class TaskModel()
{
	public bool Completed { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Details { get; set; }
	public string? Subject { get; set; }
	public DateOnly Date { get; set; } = DateOnly.MaxValue;
	public int Priority { get; set; } = 4;
	public List<SubtaskModel> Subtasks { get; set; } = [];

	public TaskModel(TaskModel taskModel) : this()
	{
		Completed = taskModel.Completed;
		Title = taskModel.Title;
		Details = taskModel.Details;
		Subject = taskModel.Subject;
		Date = taskModel.Date;
		Priority = taskModel.Priority;
		Subtasks = taskModel.Subtasks.Select(x => new SubtaskModel(x)).ToList();
	}
}

internal class SubtaskModel()
{
	public bool Completed { get; set; }
	public string Title { get; set; } = string.Empty;

	public SubtaskModel(SubtaskModel subtaskModel) : this()
	{
		Completed = subtaskModel.Completed;
		Title = subtaskModel.Title;
	}
}