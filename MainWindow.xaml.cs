using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner
{
	public partial class MainWindow : Window
	{
		private readonly DataManager dataManager;

		private readonly HomePage homePage;
		private readonly TimetablePage timetablePage;
		private readonly TasksPage tasksPage;
		private readonly EventsPage eventsPage;
		private readonly LinksPage linksPage;
		private readonly TimersPage timersPage;
		private readonly SettingsPage settingsPage;

		private readonly DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 1, 0, 0) };

		private readonly Style selectedSidePanelButtonStyle = (Style)Application.Current.FindResource("SelectedSidePanelButton");
		private readonly Style deselectedSidePanelButtonStyle = (Style)Application.Current.FindResource("DeselectedSidePanelButton");

		private readonly Thickness maximisedBorder = new(7);
		private readonly Thickness normalBorder = new(0);
		private bool sidePanelCollapsed = false;

		public MainWindow()
		{
			if (Process.GetProcessesByName("UniPlanner").Length > 1)
			{
				MessageBox.Show("Another instance of this app is already running.", "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				Application.Current.Shutdown();
			}

			InitializeComponent();
			dataManager = new();
			homePage = new HomePage() { DataManager = dataManager }.SetDisplay();
			timetablePage = new TimetablePage() { DataManager = dataManager }.SetDisplay();
			tasksPage = new TasksPage() { DataManager = dataManager }.SetDisplay();
			eventsPage = new EventsPage() { DataManager = dataManager }.SetDisplay();
			linksPage = new LinksPage() { DataManager = dataManager }.SetDisplay();
			timersPage = new TimersPage() { DataManager = dataManager }.SetDisplay();
			settingsPage = new SettingsPage() { DataManager = dataManager }.SetDisplay();

			timer.Tick += TimerUpdate;
			timer.Start();

			WindowState = dataManager.Settings.Startup ? WindowState.Normal : WindowState.Maximized;
			WindowStateChanged();

			Application.Current.Resources["ScrollBarVisibility"] = dataManager.Settings.ScrollBars ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;

			SetPage(homePage, home);
		}

		public void UpdateDisplay()
		{
			Application.Current.Resources["ScrollBarVisibility"] = dataManager.Settings.ScrollBars ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
			homePage.SetDisplay();
			timetablePage.SetDisplay();
			tasksPage.SetDisplay();
			eventsPage.SetDisplay();
			linksPage.SetDisplay();
		}

		private void SetSidePanel()
		{
			sidePanelCollapsed ^= true;

			if (sidePanelCollapsed)
			{
				SidePanel.Width = 60;
				CollapseButton.Visibility = Visibility.Collapsed;
				ExpandButton.Visibility = Visibility.Visible;
				AppIcon.Visibility = Visibility.Visible;
				AppTitle.Visibility = Visibility.Collapsed;
			}
			else
			{
				SidePanel.Width = 150;
				CollapseButton.Visibility = Visibility.Visible;
				ExpandButton.Visibility = Visibility.Collapsed;
				AppIcon.Visibility = Visibility.Collapsed;
				AppTitle.Visibility = Visibility.Visible;
			}
		}
		private void SetPage(Page page, Button selectedButton)
		{
			Title = $"UniPlanner - {selectedButton.Name}";
			PageArea.Content = page;
			PageTitle.Text = selectedButton.Name;

			foreach (Button button in SidePanelButtons.Children)
				button.Style = deselectedSidePanelButtonStyle;

			selectedButton.Style = selectedSidePanelButtonStyle;
		}
		private void TimerUpdate(object? sender, EventArgs e)
		{
			homePage.UpdateGreetingView();
			timetablePage.UpdateTimeLine();
			eventsPage.UpdateTimeLine();
		}

		private void DashboardButtonClick(object sender, RoutedEventArgs e) => SetPage(homePage, home);
		private void TimetableButtonClick(object sender, RoutedEventArgs e) => SetPage(timetablePage, timetable);
		private void TasksButtonClick(object sender, RoutedEventArgs e) => SetPage(tasksPage, tasks);
		private void TimersButtonClick(object sender, RoutedEventArgs e) => SetPage(timersPage, timers);
		private void EventsButtonClick(object sender, RoutedEventArgs e) => SetPage(eventsPage, events);
		private void LinksButtonClick(object sender, RoutedEventArgs e) => SetPage(linksPage, links);
		private void SettingsButtonClick(object sender, RoutedEventArgs e) => SetPage(settingsPage, settings);
		private void SidePanelButtonClick(object sender, RoutedEventArgs e) => SetSidePanel();

		private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();
		private void MinimiseButtonClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
		private void MaximiseButtonClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
		private void NormalButtonClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Normal;
		private void CloseButtonClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

		private void WindowStateChanged()
		{
			if (WindowState == WindowState.Maximized)
			{
				WindowBorder.Padding = maximisedBorder;
				MaximiseButton.Visibility = Visibility.Collapsed;
				NormalButton.Visibility = Visibility.Visible;
			}
			else
			{
				WindowBorder.Padding = normalBorder;
				MaximiseButton.Visibility = Visibility.Visible;
				NormalButton.Visibility = Visibility.Collapsed;
			}
		}
		private void WindowStateChanged(object sender, EventArgs e) => WindowStateChanged();
	}
}