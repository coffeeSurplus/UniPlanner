using System.Windows;
using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class TimetableViewModel : ViewModelBase
{
	private readonly DataManager<List<TimetableModel>> dataManager = ((App)Application.Current).TimetableManager;
	private readonly List<TimetableModel> timetableList;
	private readonly TimetablePdfGenerator pdfGenerator;
	private bool newTimetable = true;
	private TimetableModel currentTimetable = new();

	private int currentPageNumber;
	private bool timetableEditorVisible;
	private bool horizontalDefaultMessageVisible;
	private bool todayDefaultMessageVisible;
	private string currentTimetableTitle = string.Empty;
	private string currentTimetableDetails = string.Empty;
	private string currentTimetableSubject = string.Empty;
	private string currentTimetableLocation = string.Empty;
	private int currentTimetableDay;
	private string currentTimetableStartTime = string.Empty;
	private string currentTimetableEndTime = string.Empty;
	private int currentTimetableColour;

	public int CurrentPageNumber
	{
		get => currentPageNumber;
		set => SetValue(ref currentPageNumber, value);
	}
	public bool TimetableEditorVisible
	{
		get => timetableEditorVisible;
		set => SetValue(ref timetableEditorVisible, value);
	}
	public bool HorizontalDefaultMessageVisible
	{
		get => horizontalDefaultMessageVisible;
		set => SetValue(ref horizontalDefaultMessageVisible, value);
	}
	public bool TodayDefaultMessageVisible
	{
		get => todayDefaultMessageVisible;
		set => SetValue(ref todayDefaultMessageVisible, value);
	}
	public string CurrentTimetableTitle
	{
		get => currentTimetableTitle;
		set => SetValue(ref currentTimetableTitle, value);
	}
	public string CurrentTimetableDetails
	{
		get => currentTimetableDetails;
		set => SetValue(ref currentTimetableDetails, value);
	}
	public string CurrentTimetableSubject
	{
		get => currentTimetableSubject;
		set => SetValue(ref currentTimetableSubject, value);
	}
	public string CurrentTimetableLocation
	{
		get => currentTimetableLocation;
		set => SetValue(ref currentTimetableLocation, value);
	}
	public int CurrentTimetableDay
	{
		get => currentTimetableDay;
		set => SetValue(ref currentTimetableDay, value);
	}
	public string CurrentTimetableStartTime
	{
		get => currentTimetableStartTime;
		set => SetValue(ref currentTimetableStartTime, value);
	}
	public string CurrentTimetableEndTime
	{
		get => currentTimetableEndTime;
		set => SetValue(ref currentTimetableEndTime, value);
	}
	public int CurrentTimetableColour
	{
		get => currentTimetableColour;
		set => SetValue(ref currentTimetableColour, value);
	}

	public RelayCommand NewTimetableCommand { get; }
	public RelayCommand SaveAsPdfCommand { get; }
	public RelayCommand EditTimetableCommand { get; }
	public RelayCommand RemoveTimetableCommand { get; }
	public RelayCommand CancelEditTimetableCommand { get; }
	public RelayCommand SaveEditTimetableCommand { get; }

	public TimetableVerticalCollectionView VerticalCollectionView { get; }
	public TimetableHorizontalCollectionView HorizontalCollectionView { get; }
	public TimetableTodayCollectionView TodayCollectionView { get; }

	public TimetableViewModel()
	{
		NewTimetableCommand = new(NewTimetable);
		SaveAsPdfCommand = new(SaveAsPdf);
		EditTimetableCommand = new(EditTimetable);
		RemoveTimetableCommand = new(RemoveTimetable);
		CancelEditTimetableCommand = new(CancelEditTimetable);
		SaveEditTimetableCommand = new(SaveEditTimetable);
		timetableList = dataManager.Data;
		pdfGenerator = new(timetableList);
		VerticalCollectionView = new(timetableList);
		HorizontalCollectionView = new(timetableList);
		TodayCollectionView = new(timetableList);
		UpdateView(false);
	}

	private void NewTimetable(object parameter)
	{
		newTimetable = true;
		currentTimetable = new();
		CurrentTimetableTitle = string.Empty;
		CurrentTimetableDetails = string.Empty;
		CurrentTimetableSubject = string.Empty;
		CurrentTimetableLocation = string.Empty;
		CurrentTimetableDay = 0;
		CurrentTimetableStartTime = string.Empty;
		CurrentTimetableEndTime = string.Empty;
		CurrentTimetableColour = 0;
		TimetableEditorVisible = true;
	}
	private void SaveAsPdf(object parameter) => pdfGenerator.SavePdf();
	private void EditTimetable(object parameter)
	{
		newTimetable = false;
		currentTimetable = (TimetableModel)parameter;
		CurrentTimetableTitle = currentTimetable.Title;
		CurrentTimetableDetails = currentTimetable.Details ?? string.Empty;
		CurrentTimetableSubject = currentTimetable.Subject ?? string.Empty;
		CurrentTimetableLocation = currentTimetable.Location ?? string.Empty;
		CurrentTimetableDay = currentTimetable.Day;
		CurrentTimetableStartTime = currentTimetable.StartTime.ToString("H:mm");
		CurrentTimetableEndTime = currentTimetable.EndTime.ToString("H:mm");
		CurrentTimetableColour = currentTimetable.Colour;
		TimetableEditorVisible = true;
	}
	private void RemoveTimetable(object parameter)
	{
		timetableList.Remove((TimetableModel)parameter);
		UpdateData();
		UpdateView();
	}
	private void CancelEditTimetable(object parameter) => TimetableEditorVisible = false;
	private void SaveEditTimetable(object parameter)
	{
		if (CheckTimetableValues())
		{
			currentTimetable.Title = CurrentTimetableTitle.Trim().ToLower();
			currentTimetable.Details = !string.IsNullOrWhiteSpace(CurrentTimetableDetails) ? CurrentTimetableDetails.Trim().ToLower() : null;
			currentTimetable.Subject = !string.IsNullOrWhiteSpace(CurrentTimetableSubject) ? CurrentTimetableSubject.Trim().ToLower() : null;
			currentTimetable.Location = !string.IsNullOrWhiteSpace(CurrentTimetableLocation) ? CurrentTimetableLocation.Trim().ToLower() : null;
			currentTimetable.Day = CurrentTimetableDay;
			currentTimetable.StartTime = TimeOnly.Parse(CurrentTimetableStartTime);
			currentTimetable.EndTime = TimeOnly.Parse(CurrentTimetableEndTime);
			currentTimetable.Colour = CurrentTimetableColour;
			if (newTimetable)
			{
				timetableList.Add(currentTimetable);
			}
			TimetableEditorVisible = false;
			UpdateData();
			UpdateView();
		}
	}

	private bool CheckTimetableValues()
	{
		if (!string.IsNullOrWhiteSpace(CurrentTimetableTitle))
		{
			if (TimeOnly.TryParse(CurrentTimetableStartTime, out TimeOnly newStartTime))
			{
				if (newStartTime.ToTimeSpan().TotalMinutes >= 540)
				{
					if (TimeOnly.TryParse(CurrentTimetableEndTime, out TimeOnly newEndTime))
					{
						if (newEndTime.ToTimeSpan().TotalMinutes <= 960)
						{
							if (newEndTime.CompareTo(newStartTime) == 1 && (newEndTime - newStartTime).TotalMinutes >= 15)
							{
								if (timetableList.All(x => x == currentTimetable || x.Day != CurrentTimetableDay || x.StartTime >= newEndTime || x.EndTime <= newStartTime))
								{
									return true;
								}
								else
								{
									MessageBox.Show("Event must not overlap with other events.", "UniPlanner");
								}
							}
							else
							{
								MessageBox.Show("Event must be at least 15 minutes long.", "UniPlanner");
							}
						}
						else
						{
							MessageBox.Show("End time must be before 16:00.", "UniPlanner");
						}
					}
					else
					{
						MessageBox.Show("Invalid end time.", "UniPlanner");
					}
				}
				else
				{
					MessageBox.Show("Start time must be after 09:00.", "UniPlanner");
				}
			}
			else
			{
				MessageBox.Show("Invalid start time.", "UniPlanner");
			}
		}
		else
		{
			MessageBox.Show("Invalid title.", "UniPlanner");
		}
		TimetableEditorVisible = true;
		return false;
	}
	private void UpdateData() => dataManager.UpdateData();
	private void UpdateView(bool updateHomeView = true)
	{
		VerticalCollectionView.UpdateView();
		HorizontalCollectionView.UpdateView();
		TodayCollectionView.UpdateView();
		HorizontalDefaultMessageVisible = timetableList.Count == 0;
		TodayDefaultMessageVisible = timetableList.All(x => x.Day != DateOnly.FromDateTime(DateTime.Now).UKDayOfWeek());
		if (updateHomeView)
		{
			((App)Application.Current).UpdateHomeView();
		}
	}
}