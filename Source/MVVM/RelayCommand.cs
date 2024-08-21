using System.Windows.Input;

namespace UniPlanner.Source.MVVM;

internal class RelayCommand(Action execute) : ICommand
{
	private readonly Action execute = execute;

	public event EventHandler? CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}

	public void Execute() => execute.Invoke();
	public void Execute(object? parameter) => execute.Invoke();
	public bool CanExecute(object? parameter) => true;
}

internal class RelayCommand<T>(Action<T> execute) : ICommand
{
	private readonly Action<T> execute = execute;

	public event EventHandler? CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}

	public void Execute(T parameter) => execute.Invoke(parameter);
	public void Execute(object? parameter) => execute.Invoke((T)parameter!);
	public bool CanExecute(object? parameter) => true;
}