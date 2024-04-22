using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using UniPlanner.Classes;

namespace UniPlanner.Pages
{
	public partial class SettingsPage : Page
	{
		public required DataManager DataManager { get; init; }

		private readonly Style selectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style deselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");

		private SoundPlayer? alarm;

		public SettingsPage() => InitializeComponent();

		public SettingsPage SetDisplay()
		{
			SetStartupButtons(DataManager.Settings.Startup ? NormalWindow : MaximisedWindow);
			SetUsernameTextBox(DataManager.Settings.Username);
			SetScrollBarsButtons(DataManager.Settings.ScrollBars ? Visible : Hidden);
			SetAlarmButtons(DataManager.Settings.AlarmSound switch
			{
				"Xylophone" => Xylophone,
				"Chords" => Chords,
				"Tap" => Tap,
				"Jingle" => Jingle,
				"Transition" => Transition,
				"Descending" => Descending,
				"Bounce" => Bounce,
				"Echo" => Echo,
				"Ascending" => Ascending,
				_ => Chimes
			});

			SetBrowserButtons(DataManager.Settings.Browser switch
			{
				"chrome" => Chrome,
				"firefox" => Firefox,
				"opera" => Opera,
				_ => Msedge
			});

			SetPrivateBrowsingButtons(DataManager.Settings.PrivateBrowsing ? PrivateBrowsing : NormalBrowsing);
			SetPdfSaveButtons(DataManager.Settings.PdfSave switch
			{
				"browser" => Browser,
				"explorer" => Explorer,
				"none" => None,
				_ => Default
			});

			return this;
		}

		private void SetStartupButtons(Button selectedButton)
		{
			if (selectedButton.Name == "NormalWindow")
			{
				NormalWindow.Style = selectedButtonStyle;
				MaximisedWindow.Style = deselectedButtonStyle;
			}
			else
			{
				NormalWindow.Style = deselectedButtonStyle;
				MaximisedWindow.Style = selectedButtonStyle;
			}
		}
		private void SetStartup(Button selectedButton)
		{
			SetStartupButtons(selectedButton);
			DataManager.Settings.Startup = selectedButton.Name == "NormalWindow";
			DataManager.UpdateSettings();
			((MainWindow)Application.Current.MainWindow).UpdateDisplay();
		}
		private void StartupButtonClick(object sender, RoutedEventArgs e) => SetStartup((Button)sender);

		private void SetUsernameTextBox(string username) => Username.Text = username.Trim().ToLower();
		private void SetUsernameName(string username)
		{
			SetUsernameTextBox(username);
			DataManager.Settings.Username = username.Trim().ToLower();
			DataManager.UpdateSettings();
			((MainWindow)Application.Current.MainWindow).UpdateDisplay();
		}
		private void UsernameChanged(object sender, RoutedEventArgs e) => SetUsernameName(((TextBox)sender).Text);

		private void SetScrollBarsButtons(Button selectedButton)
		{
			if (selectedButton.Name == "Visible")
			{
				Visible.Style = selectedButtonStyle;
				Hidden.Style = deselectedButtonStyle;
			}
			else
			{
				Visible.Style = deselectedButtonStyle;
				Hidden.Style = selectedButtonStyle;
			}
		}
		private void SetScrollBars(Button selectedButton)
		{
			SetScrollBarsButtons(selectedButton);
			DataManager.Settings.ScrollBars = selectedButton.Name == "Visible";
			DataManager.UpdateSettings();
			((MainWindow)Application.Current.MainWindow).UpdateDisplay();
		}
		private void ScrollBarsButtonClick(object sender, RoutedEventArgs e) => SetScrollBars((Button)sender);

		private void SetAlarmButtons(Button selectedButton)
		{
			Xylophone.Style = deselectedButtonStyle;
			Chords.Style = deselectedButtonStyle;
			Tap.Style = deselectedButtonStyle;
			Jingle.Style = deselectedButtonStyle;
			Transition.Style = deselectedButtonStyle;
			Descending.Style = deselectedButtonStyle;
			Bounce.Style = deselectedButtonStyle;
			Echo.Style = deselectedButtonStyle;
			Ascending.Style = deselectedButtonStyle;
			Chimes.Style = deselectedButtonStyle;

			selectedButton.Style = selectedButtonStyle;
		}
		private void SetAlarm(Button selectedButton)
		{
			SetAlarmButtons(selectedButton);
			DataManager.Settings.AlarmSound = selectedButton.Name;
			DataManager.UpdateSettings();
		}
		private void AlarmSoundButtonClick(object sender, RoutedEventArgs e)
		{
			SetAlarm((Button)sender);
			alarm?.Dispose();
			alarm = new(Assembly.GetExecutingAssembly()
				.GetManifestResourceStream($"UniPlanner.Resources.Alarms.{DataManager.Settings.AlarmSound}.wav"));
			alarm.Play();
		}

		private void SetBrowserButtons(Button selectedButton)
		{
			Msedge.Style = deselectedButtonStyle;
			Chrome.Style = deselectedButtonStyle;
			Firefox.Style = deselectedButtonStyle;
			Opera.Style = deselectedButtonStyle;
			selectedButton.Style = selectedButtonStyle;
		}
		private void SetBrowser(Button selectedButton)
		{
			SetBrowserButtons(selectedButton);
			DataManager.Settings.Browser = selectedButton.Name.ToLower();
			DataManager.UpdateSettings();
		}
		private void BrowserButtonClick(object sender, RoutedEventArgs e) => SetBrowser((Button)sender);

		private void SetPrivateBrowsingButtons(Button selectedButton)
		{
			if (selectedButton.Name == "PrivateBrowsing")
			{
				PrivateBrowsing.Style = selectedButtonStyle;
				NormalBrowsing.Style = deselectedButtonStyle;
			}
			else
			{
				PrivateBrowsing.Style = deselectedButtonStyle;
				NormalBrowsing.Style = selectedButtonStyle;
			}
		}
		private void SetPrivateBrowsing(Button selectedButton)
		{
			SetPrivateBrowsingButtons(selectedButton);
			DataManager.Settings.PrivateBrowsing = selectedButton.Name == "PrivateBrowsing";
			DataManager.UpdateSettings();
		}
		private void PrivateBrowsingButtonClick(object sender, RoutedEventArgs e) => SetPrivateBrowsing((Button)sender);

		private void SetPdfSaveButtons(Button selectedButton)
		{
			Default.Style = deselectedButtonStyle;
			Browser.Style = deselectedButtonStyle;
			Explorer.Style = deselectedButtonStyle;
			None.Style = deselectedButtonStyle;
			selectedButton.Style = selectedButtonStyle;
		}
		private void SetPdfSave(Button selectedButton)
		{
			SetPdfSaveButtons(selectedButton);
			DataManager.Settings.PdfSave = selectedButton.Name.ToLower();
			DataManager.UpdateSettings();
		}
		private void PdfSaveButtonClick(object sender, RoutedEventArgs e) => SetPdfSave((Button)sender);
	}
}