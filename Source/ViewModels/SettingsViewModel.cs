using System.Media;
using System.Windows;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class SettingsViewModel : ViewModelBase
{
	private readonly DataManager<SettingsModel> dataManager = ((App)Application.Current).SettingsManager;
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
			UpdateData();
			((App)Application.Current).UpdateHomeView();
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

	public RelayCommand UpdateDataCommand { get; }
	public RelayCommand SetScrollbarsCommand { get; }
	public RelayCommand PlayAudioCommand { get; }

	public SettingsViewModel()
	{
		UpdateDataCommand = new(UpdateData);
		SetScrollbarsCommand = new(SetScrollbars);
		PlayAudioCommand = new(PlayAudio);
		startupMaximised = dataManager.Data.StartupMaximised;
		username = dataManager.Data.Username;
		scrollbarsEnabled = dataManager.Data.ScrollbarsEnabled;
		alarmSound = dataManager.Data.AlarmSound;
		browser = dataManager.Data.Browser;
		privateBrowsing = dataManager.Data.PrivateBrowsing;
		pdfSaveAction = dataManager.Data.PdfSaveAction;
	}

	private void UpdateData(object parameter) => UpdateData();
	private void SetScrollbars(object parameter)
	{
		UpdateData();
		((App)Application.Current).SetScrollbars();
	}
	private void PlayAudio(object parameter)
	{
		UpdateData();
		soundPlayer.Stop();
		soundPlayer = dataManager.Data.ReturnAlarm();
		soundPlayer.Play();
	}

	private void UpdateData()
	{
		dataManager.Data.StartupMaximised = StartupMaximised;
		dataManager.Data.Username = Username.Trim().ToLower();
		dataManager.Data.ScrollbarsEnabled = ScrollbarsEnabled;
		dataManager.Data.AlarmSound = AlarmSound;
		dataManager.Data.Browser = Browser;
		dataManager.Data.PrivateBrowsing = PrivateBrowsing;
		dataManager.Data.PdfSaveAction = PdfSaveAction;
		dataManager.UpdateData();
	}
}