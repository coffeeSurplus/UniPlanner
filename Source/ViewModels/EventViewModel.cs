using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class EventViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;
	private readonly List<EventModel> eventList;
	private bool newEvent = true;
	private EventModel currentEvent = new();

	private int currentPageNumber = 0;
	private int monthEventCount = 0;
	private int weekEventCount = 0;
	private int dayEventCount = 0;
	private bool eventEditorVisible = false;
	private DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
	private string currentEventTitle = string.Empty;
	private string currentEventDetails = string.Empty;
	private string currentEventLocation = string.Empty;
	private string currentEventDate = string.Empty;
	private bool currentEventAllDay = false;
	private string currentEventStartTime = string.Empty;
	private string currentEventEndTime = string.Empty;
	private int currentEventColour = 0;

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
	public RelayCommand CancelEditEventCommand { get; }
	public RelayCommand SaveEditEventCommand { get; }
	public RelayCommand<EventModel> EditEventCommand { get; }
	public RelayCommand<EventModel> RemoveEventCommand { get; }

	public EventMonthCollectionView MonthCollectionView { get; }
	public EventWeekCollectionView WeekCollectionView { get; }
	public EventDayCollectionView DayCollectionView { get; }

	public EventViewModel(DataAccess data)
	{
		dataAccess = data;
		eventList = dataAccess.EventList;
		MonthCollectionView = new(eventList);
		WeekCollectionView = new(eventList);
		DayCollectionView = new(eventList);
		SetTodayCommand = new(SetToday);
		IncreaseMonthCommand = new(IncreaseMonth);
		DecreaseMonthCommand = new(DecreaseMonth);
		IncreaseWeekCommand = new(IncreaseWeek);
		DecreaseWeekCommand = new(DecreaseWeek);
		IncreaseDayCommand = new(IncreaseDay);
		DecreaseDayCommand = new(DecreaseDay);
		NewEventCommand = new(NewEvent);
		CancelEditEventCommand = new(CancelEditEvent);
		SaveEditEventCommand = new(SaveEditEvent);
		EditEventCommand = new(EditEvent);
		RemoveEventCommand = new(RemoveEvent);
		UpdateView(false);
	}

	private void SetToday()
	{
		CurrentDate = DateOnly.FromDateTime(DateTime.Now);
		UpdateView(true);
	}
	private void IncreaseMonth()
	{
		CurrentDate = CurrentDate.AddMonths(1);
		UpdateView(true);
	}
	private void DecreaseMonth()
	{
		CurrentDate = CurrentDate.AddMonths(-1);
		UpdateView(true);
	}
	private void IncreaseWeek()
	{
		CurrentDate = CurrentDate.AddDays(7);
		UpdateView(true);
	}
	private void DecreaseWeek()
	{
		CurrentDate = CurrentDate.AddDays(-7);
		UpdateView(true);
	}
	private void IncreaseDay()
	{
		CurrentDate = CurrentDate.AddDays(1);
		UpdateView(true);
	}
	private void DecreaseDay()
	{
		CurrentDate = CurrentDate.AddDays(-1);
		UpdateView(true);
	}
	private void NewEvent()
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
	private void CancelEditEvent() => EventEditorVisible = false;
	private void SaveEditEvent()
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
			UpdateView(true);
		}
	}
	private void EditEvent(EventModel parameter)
	{
		newEvent = false;
		currentEvent = parameter;
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
	private void RemoveEvent(EventModel parameter)
	{
		eventList.Remove(parameter);
		UpdateData();
		UpdateView(true);
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
						Popup.MessageBox("Cannot have multiple all-day events on the same day.");
					}
				}
				else if (TimeOnly.TryParse(CurrentEventStartTime, out TimeOnly newStartTime))
				{
					if (TimeOnly.TryParse(CurrentEventEndTime, out TimeOnly newEndTime))
					{
						if (newEndTime.CompareTo(newStartTime) == 1 && (newEndTime - newStartTime).TotalMinutes >= 15)
						{
							if (eventList.All(x => x == currentEvent || x.Date != newDate || x.AllDay || x.StartTime >= newEndTime || x.EndTime <= newStartTime))
							{
								return true;
							}
							else
							{
								Popup.MessageBox("Event must not overlap with other events.");
							}
						}
						else
						{
							Popup.MessageBox("Event must be at least 15 minutes long.");
						}
					}
					else
					{
						Popup.MessageBox("Invalid end time.");
					}
				}
				else
				{
					Popup.MessageBox("Invalid start time.");
				}
			}
			else
			{
				Popup.MessageBox("Invalid date.");
			}
		}
		else
		{
			Popup.MessageBox("Invalid title.");
		}
		EventEditorVisible = true;
		return false;
	}
	private void UpdateData() => dataAccess.UpdateEventList();
	private void UpdateView(bool updateHomeView)
	{
		MonthCollectionView.UpdateView(eventList, CurrentDate);
		WeekCollectionView.UpdateView(CurrentDate);
		DayCollectionView.UpdateView(CurrentDate);
		MonthEventCount = eventList.Count(x => (x.Date.Year, x.Date.Month) == (CurrentDate.Year, CurrentDate.Month));
		WeekEventCount = eventList.Count(x => x.Date.FirstDayOfWeek() == CurrentDate.FirstDayOfWeek());
		DayEventCount = eventList.Count(x => x.Date == CurrentDate);
		if (updateHomeView)
		{
			DataAccess.MainWindow.UpdateHomeView();
		}
	}
}