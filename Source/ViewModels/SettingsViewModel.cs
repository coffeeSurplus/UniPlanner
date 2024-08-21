using System.Media;
using UniPlanner.Source.Data;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class SettingsViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;
	private SoundPlayer soundPlayer = new();

	private bool startupMaximised;
	private string username;
	private bool scrollbarsEnabled;
	private int alarmSound;
	private int browser;
	private bool privateBrowsing;
	private int pdfSaveAction;

	public bool StartupMaximised
	{
		get => startupMaximised;
		set => SetValue(ref startupMaximised, value);
	}
	public string Username
	{
		get => username;
		set
		{
			SetValue(ref username, value);
			UpdateUsername();
		}
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

	public RelayCommand UpdateSettingsCommand { get; }
	public RelayCommand SetScrollbarsCommand { get; }
	public RelayCommand PlayAudioCommand { get; }

	public SettingsViewModel(DataAccess data)
	{
		dataAccess = data;
		startupMaximised = dataAccess.Settings.StartupMaximised;
		username = dataAccess.Settings.Username;
		scrollbarsEnabled = dataAccess.Settings.ScrollbarsEnabled;
		alarmSound = dataAccess.Settings.AlarmSound;
		browser = dataAccess.Settings.Browser;
		privateBrowsing = dataAccess.Settings.PrivateBrowsing;
		pdfSaveAction = dataAccess.Settings.PdfSaveAction;
		UpdateSettingsCommand = new(UpdateSettings);
		SetScrollbarsCommand = new(SetScrollbars);
		PlayAudioCommand = new(PlayAudio);
	}

	private void UpdateSettings() => UpdateData();
	private void SetScrollbars()
	{
		UpdateSettings();
		DataAccess.MainWindow.SetScrollbars();
	}
	private void PlayAudio()
	{
		UpdateSettings();
		soundPlayer.Stop();
		soundPlayer = dataAccess.Settings.ReturnAlarm();
		soundPlayer.Play();
	}

	private void UpdateUsername()
	{
		UpdateSettings();
		DataAccess.MainWindow.UpdateHomeView();
	}
	private void UpdateData()
	{
		dataAccess.Settings.StartupMaximised = StartupMaximised;
		dataAccess.Settings.Username = Username.Trim().ToLower();
		dataAccess.Settings.ScrollbarsEnabled = ScrollbarsEnabled;
		dataAccess.Settings.AlarmSound = AlarmSound;
		dataAccess.Settings.Browser = Browser;
		dataAccess.Settings.PrivateBrowsing = PrivateBrowsing;
		dataAccess.Settings.PdfSaveAction = PdfSaveAction;
		dataAccess.UpdateSettings();
	}
}