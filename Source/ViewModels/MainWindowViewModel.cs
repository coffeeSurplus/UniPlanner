using System.Windows;
using System.Windows.Controls;
using UniPlanner.Source.Data;
using UniPlanner.Source.MVVM;
using UniPlanner.Source.Views;

namespace UniPlanner.Source.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
	private readonly Page[] pages = [new HomeView(), new TimetableView(), new TaskView(), new EventView(), new LinkView(), new TimersView(), new SettingsView()];

	private WindowState currentState = MainProgram.SettingsManager.Data.StartupMaximised ? WindowState.Maximized : WindowState.Normal;
	private bool sidepanelCollapsed = false;
	private int currentPageNumber = 0;
	private Page currentPage;

	public WindowState CurrentState
	{
		get => currentState;
		set => SetValue(ref currentState, value);
	}
	public bool SidepanelCollapsed
	{
		get => sidepanelCollapsed;
		set => SetValue(ref sidepanelCollapsed, value);
	}
	public int CurrentPageNumber
	{
		get => currentPageNumber;
		set => SetValue(ref currentPageNumber, value);
	}
	public Page CurrentPage
	{
		get => currentPage;
		set => SetValue(ref currentPage, value);
	}

	public RelayCommand SetPageCommand { get; }
	public RelayCommand MinimiseWindowCommand { get; }
	public RelayCommand MaximiseWindowCommand { get; }
	public RelayCommand CloseWindowCommand { get; }

	public MainWindowViewModel()
	{
		SetPageCommand = new(SetPage);
		MinimiseWindowCommand = new(MinimiseWindow);
		MaximiseWindowCommand = new(MaximiseWindow);
		CloseWindowCommand = new(CloseWindow);
		currentPage = pages[0];
	}

	public void UpdateHomeView() => ((HomeViewModel)pages[0].DataContext).UpdateView();

	private void SetPage(object parameer) => CurrentPage = pages[CurrentPageNumber];
	private void MinimiseWindow(object parameter) => CurrentState = WindowState.Minimized;
	private void MaximiseWindow(object parameter) => CurrentState = CurrentState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
	private void CloseWindow(object parameter) => Application.Current.Shutdown();
}