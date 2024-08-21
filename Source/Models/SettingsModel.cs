using System.Media;
using UniPlanner.Properties;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.Models;

internal class SettingsModel : ViewModelBase
{
	private bool startupMaximised = false;
	private string username = "new user";
	private bool scrollbarsEnabled = true;
	private int alarmSound = 0;
	private int browser = 0;
	private bool privateBrowsing = false;
	private int pdfSaveAction = 0;

	public bool StartupMaximised
	{
		get => startupMaximised;
		set => SetValue(ref startupMaximised, value);
	}
	public string Username
	{
		get => username;
		set => SetValue(ref username, value);
	}
	public bool ScrollbarsEnabled
	{
		get => scrollbarsEnabled;
		set => SetValue(ref scrollbarsEnabled, value);
	}
	public int AlarmSound
	{
		get => alarmSound;
		set => SetValue(ref alarmSound, value);
	}
	public int Browser
	{
		get => browser;
		set => SetValue(ref browser, value);
	}
	public bool PrivateBrowsing
	{
		get => privateBrowsing;
		set => SetValue(ref privateBrowsing, value);
	}
	public int PdfSaveAction
	{
		get => pdfSaveAction;
		set => SetValue(ref pdfSaveAction, value);
	}

	public SoundPlayer ReturnAlarm() => new(AlarmSound switch { 0 => Resources.Ascending, 1 => Resources.Bounce, 2 => Resources.Chimes, 3 => Resources.Chords, 4 => Resources.Descending, 5 => Resources.Echo, 6 => Resources.Jingle, 7 => Resources.Tap, 8 => Resources.Transition, _ => Resources.Xylophone });
	public string ReturnBrowser() => Browser switch { 0 => "msedge", 1 => "chrome", 2 => "firefox", _ => "opera" };
	public string ReturnArguments() => PrivateBrowsing ? Browser switch { 0 => "-inprivate ", 2 => "-incognito ", 3 => "-private-window ", _ => "-private " } : string.Empty;
}