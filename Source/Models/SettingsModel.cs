using System.Media;
using UniPlanner.Properties;

namespace UniPlanner.Source.Models;

internal class SettingsModel
{
	public bool StartupMaximised { get; set; }
	public string Username { get; set; } = "new user";
	public bool ScrollbarsEnabled { get; set; } = true;
	public int AlarmSound { get; set; }
	public int Browser { get; set; }
	public bool PrivateBrowsing { get; set; }
	public int PdfSaveAction { get; set; }

	public SoundPlayer ReturnAlarm() => new(AlarmSound switch { 0 => Resources.Ascending, 1 => Resources.Bounce, 2 => Resources.Chimes, 3 => Resources.Chords, 4 => Resources.Descending, 5 => Resources.Echo, 6 => Resources.Jingle, 7 => Resources.Tap, 8 => Resources.Transition, _ => Resources.Xylophone });
	public string ReturnBrowser() => Browser switch { 0 => "msedge", 1 => "chrome", 2 => "firefox", _ => "opera" };
	public string ReturnArguments() => PrivateBrowsing ? Browser switch { 0 => "-inprivate ", 2 => "-incognito ", 3 => "-private-window ", _ => "-private " } : string.Empty;
}