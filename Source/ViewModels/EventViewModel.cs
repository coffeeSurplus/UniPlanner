using System.Windows;
using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class EventViewModel : ViewModelBase
{
	private readonly DataManager<List<EventModel>> dataManager = ((App)Application.Current).EventManager;
	private readonly List<EventModel> eventList;
	private bool newEvent;
	private EventModel currentEvent = new();

	private int currentPageNumber;
	private int monthEventCount;
	private int weekEventCount;
	private int dayEventCount;
	private bool eventEditorVisible;
	private DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
	private string currentEventTitle = string.Empty;
	private string currentEventDetails = string.Empty;
	private string currentEventLocation = string.Empty;
	private string currentEventDate = string.Empty;
	private bool currentEventAllDay;
	private string currentEventStartTime = string.Empty;
	private string currentEventEndTime = string.Empty;
	private int currentEventColour;

	public int CurrentPageNumber
	{
		get => currentPageNumber;
		set => SetValue(ref currentPageNumber, value);
	}
	public int MonthEventCount
	{
		get => monthEventCount;
		set => SetValue(ref monthEventCount, value);
	}
	public int WeekEventCount
	{
		get => weekEventCount;
		set => SetValue(ref weekEventCount, value);
	}
	public int DayEventCount
	{
		get => dayEventCount;
		set => SetValue(ref dayEventCount, value);
	}
	public bool EventEditorVisible
	{
		get => eventEditorVisible;
		set => SetValue(ref eventEditorVisible, value);
	}
	public DateOnly CurrentDate
	{
		get => currentDate;
		set => SetValue(ref currentDate, value);
	}
	public string CurrentEventTitle
	{
		get => currentEventTitle;
		set => SetValue(ref currentEventTitle, value);
	}
	public string CurrentEventDetails
	{
		get => currentEventDetails;
		set => SetValue(ref currentEventDetails, value);
	}
	public string CurrentEventLocation
	{
		get => currentEventLocation;
		set => SetValue(ref currentEventLocation, value);
	}
	public string CurrentEventDate
	{
		get => currentEventDate;
		set => SetValue(ref currentEventDate, value);
	}
	public bool CurrentEventAllDay
	{
		get => currentEventAllDay;
		set => SetValue(ref currentEventAllDay, value);
	}
	public string CurrentEventStartTime
	{
		get => currentEventStartTime;
		set => SetValue(ref currentEventStartTime, value);
	}
	public string CurrentEventEndTime
	{
		get => currentEventEndTime;
		set => SetValue(ref currentEventEndTime, value);
	}
	public int CurrentEventColour
	{
		get => currentEventColour;
		set => SetValue(ref currentEventColour, value);
	}

	public RelayCommand SetTodayCommand { get; }
	public RelayCommand IncreaseMonthCommand { get; }
	public RelayCommand DecreaseMonthCommand { get; }
	public RelayCommand IncreaseWeekCommand { get; }
	public RelayCommand DecreaseWeekCommand { get; }
	public RelayCommand IncreaseDayCommand { get; }
	public RelayCommand DecreaseDayCommand { get; }
	public RelayCommand NewEventCommand { get; }
	public RelayCommand EditEventCommand { get; }
	public RelayCommand CancelEditEventCommand { get; }
	public RelayCommand SaveEditEventCommand { get; }
	public RelayCommand RemoveEventCommand { get; }

	public EventMonthCollectionView MonthCollectionView { get; }
	public EventWeekCollectionView WeekCollectionView { get; }
	public EventDayCollectionView DayCollectionView { get; }

	public EventViewModel()
	{
		SetTodayCommand = new(SetToday);
		IncreaseMonthCommand = new(IncreaseMonth);
		DecreaseMonthCommand = new(DecreaseMonth);
		IncreaseWeekCommand = new(IncreaseWeek);
		DecreaseWeekCommand = new(DecreaseWeek);
		IncreaseDayCommand = new(IncreaseDay);
		DecreaseDayCommand = new(DecreaseDay);
		NewEventCommand = new(NewEvent);
		EditEventCommand = new(EditEvent);
		CancelEditEventCommand = new(CancelEditEvent);
		SaveEditEventCommand = new(SaveEditEvent);
		RemoveEventCommand = new(RemoveEvent);
		eventList = dataManager.Data;
		MonthCollectionView = new(eventList, CurrentDate);
		WeekCollectionView = new(eventList, CurrentDate);
		DayCollectionView = new(eventList, CurrentDate);
		UpdateView(false);
	}

	private void SetToday(object parameter)
	{
		CurrentDate = DateOnly.FromDateTime(DateTime.Now);
		UpdateView();
	}
	private void IncreaseMonth(object parameter)
	{
		CurrentDate = CurrentDate.AddMonths(1);
		UpdateView();
	}
	private void DecreaseMonth(object parameter)
	{
		CurrentDate = CurrentDate.AddMonths(-1);
		UpdateView();
	}
	private void IncreaseWeek(object parameter)
	{
		CurrentDate = CurrentDate.AddDays(7);
		UpdateView();
	}
	private void DecreaseWeek(object parameter)
	{
		CurrentDate = CurrentDate.AddDays(-7);
		UpdateView();
	}
	private void IncreaseDay(object parameter)
	{
		CurrentDate = CurrentDate.AddDays(1);
		UpdateView();
	}
	private void DecreaseDay(object parameter)
	{
		CurrentDate = CurrentDate.AddDays(-1);
		UpdateView();
	}
	private void NewEvent(object parameter)
	{
		newEvent = true;
		currentEvent = new();
		CurrentEventTitle = string.Empty;
		CurrentEventDetails = string.Empty;
		CurrentEventLocation = string.Empty;
		CurrentEventDate = string.Empty;
		CurrentEventAllDay = false;
		CurrentEventStartTime = string.Empty;
		CurrentEventEndTime = string.Empty;
		CurrentEventColour = 0;
		EventEditorVisible = true;
	}
	private void EditEvent(object parameter)
	{
		newEvent = false;
		currentEvent = (EventModel)parameter;
		CurrentEventTitle = currentEvent.Title;
		CurrentEventDetails = currentEvent.Details ?? string.Empty;
		CurrentEventLocation = currentEvent.Location ?? string.Empty;
		CurrentEventDate = currentEvent.Date.ToString("d/M");
		CurrentEventAllDay = currentEvent.AllDay;
		CurrentEventStartTime = !currentEvent.AllDay ? currentEvent.StartTime.ToString("H:mm") : string.Empty;
		CurrentEventEndTime = !currentEvent.AllDay ? currentEvent.EndTime.ToString("H:mm") : string.Empty;
		CurrentEventColour = currentEvent.Colour;
		EventEditorVisible = true;
	}
	private void CancelEditEvent(object parameter) => EventEditorVisible = false;
	private void SaveEditEvent(object parameter)
	{
		if (CheckEventValues())
		{
			currentEvent.Title = CurrentEventTitle.Trim().ToLower();
			currentEvent.Details = !string.IsNullOrWhiteSpace(CurrentEventDetails) ? CurrentEventDetails : null;
			currentEvent.Location = !string.IsNullOrWhiteSpace(CurrentEventLocation) ? CurrentEventLocation : null;
			currentEvent.Date = DateOnly.Parse(CurrentEventDate);
			currentEvent.AllDay = CurrentEventAllDay;
			currentEvent.StartTime = CurrentEventAllDay ? TimeOnly.MinValue : TimeOnly.Parse(CurrentEventStartTime);
			currentEvent.EndTime = CurrentEventAllDay ? TimeOnly.MaxValue : TimeOnly.Parse(CurrentEventEndTime);
			currentEvent.Colour = CurrentEventColour;
			if (newEvent)
			{
				eventList.Add(currentEvent);
			}
			EventEditorVisible = false;
			UpdateData();
			UpdateView();
		}
	}
	private void RemoveEvent(object parameter)
	{
		eventList.Remove((EventModel)parameter);
		UpdateData();
		UpdateView();
	}

	private bool CheckEventValues()
	{
		if (!string.IsNullOrWhiteSpace(CurrentEventTitle))
		{
			if (DateOnly.TryParse(CurrentEventDate, out DateOnly newDate))
			{
				if (CurrentEventAllDay)
				{
					if (eventList.All(x => x == currentEvent || !x.AllDay || x.Date != newDate))
					{
						return true;
					}
					else
					{
						MessageBox.Show("Cannot have multiple all-day events on the same day.", "UniPlanner");
					}
				}
				else if (TimeOnly.TryParse(currentEventStartTime, out TimeOnly newStartTime))
				{
					if (TimeOnly.TryParse(currentEventEndTime, out TimeOnly newEndTime))
					{
						if (newEndTime.CompareTo(newStartTime) == 1 && (newEndTime - newStartTime).TotalMinutes >= 15)
						{
							if (eventList.All(x => x == currentEvent || x.Date != newDate || x.AllDay || x.StartTime >= newEndTime || x.EndTime <= newStartTime))
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
						MessageBox.Show("Invalid end time.", "UniPlanner");
					}
				}
				else
				{
					MessageBox.Show("Invalid start time.", "UniPlanner");
				}
			}
			else
			{
				MessageBox.Show("Invalid date.", "UniPlanner");
			}
		}
		else
		{
			MessageBox.Show("Invalid title.", "UniPlanner");
		}
		EventEditorVisible = true;
		return false;
	}
	private void UpdateData() => dataManager.UpdateData();
	private void UpdateView(bool updateHomeView = true)
	{
		MonthCollectionView.UpdateView(eventList, CurrentDate);
		WeekCollectionView.UpdateView(CurrentDate);
		DayCollectionView.UpdateView(CurrentDate);
		MonthEventCount = eventList.Count(x => (x.Date.Year, x.Date.Month) == (currentDate.Year, CurrentDate.Month));
		WeekEventCount = eventList.Count(x => x.Date.FirstDayOfWeek() == currentDate.FirstDayOfWeek());
		DayEventCount = eventList.Count(x => x.Date == currentDate);
		if (updateHomeView)
		{
			((App)Application.Current).UpdateHomeView();
		}
	}
}