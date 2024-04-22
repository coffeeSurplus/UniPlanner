using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.EventControls
{
	public partial class EventEditor : UserControl
	{
		public required EventsPage EventsPage { get; init; }

		private readonly Style SelectedColourButtonStyle = (Style)Application.Current.FindResource("SelectedColourButton");
		private readonly Style DeselectedColourButtonStyle = (Style)Application.Current.FindResource("DeselectedColourButton");

		private Event @event = new() { Title = string.Empty, Date = DateOnly.FromDateTime(DateTime.Now), Colour = 0 };
		private int selectedColour = 0;
		private bool newEvent = false;

		public EventEditor() => InitializeComponent();

		public void SetDefaultDisplay()
		{
			@event = new() { Title = string.Empty, Date = DateOnly.FromDateTime(DateTime.Now), Colour = 0 };
			TitleInput.Text = string.Empty;
			DetailsInput.Text = string.Empty;
			LocationInput.Text = string.Empty;
			DateInput.Text = string.Empty;
			StartTimeInput.Text = string.Empty;
			EndTimeInput.Text = string.Empty;
			selectedColour = 0;
			newEvent = true;
			SetColourButtons(WhiteButton);
		}
		public void SetDisplay(Event @event)
		{
			this.@event = @event;
			TitleInput.Text = @event.Title;
			DetailsInput.Text = @event.Details;
			LocationInput.Text = @event.Location;
			DateInput.Text = $"{@event.Date:d/M}";
			StartTimeInput.Text = @event.StartTime != null ? $"{(TimeOnly)@event.StartTime:H:mm}" : String.Empty;
			EndTimeInput.Text = @event.EndTime != null ? $"{(TimeOnly)@event.EndTime:H:mm}" : String.Empty;
			selectedColour = 0;
			newEvent = false;

			SetColourButtons(@event.Colour switch
			{
				1 => RedButton,
				2 => OrangeButton,
				3 => YellowButton,
				4 => GreenButton,
				5 => BlueButton,
				6 => IndigoButton,
				7 => PurpleButton,
				8 => PinkButton,
				_ => WhiteButton
			});
		}
		public void FocusKeyboard() => Keyboard.Focus(TitleInput);

		private void SetColourButtons(Button selectedButton)
		{
			foreach (Button button in ColourButtons.Children)
				button.Style = DeselectedColourButtonStyle;

			selectedButton.Style = SelectedColourButtonStyle;

			selectedColour = selectedButton.Name switch
			{
				"RedButton" => 1,
				"OrangeButton" => 2,
				"YellowButton" => 3,
				"GreenButton" => 4,
				"BlueButton" => 5,
				"IndigoButton" => 6,
				"PurpleButton" => 7,
				"PinkButton" => 8,
				_ => 0
			};
		}
		private void ColourButtonClick(object sender, RoutedEventArgs e) => SetColourButtons((Button)sender);

		private bool CheckInputs()
		{
			List<string> errorList = [];

			if (string.IsNullOrWhiteSpace(TitleInput.Text))
				errorList.Add("Title must not be empty.");

			if (string.IsNullOrWhiteSpace(DateInput.Text) || !DateOnly.TryParse(DateInput.Text, out _))
				errorList.Add("Invalid date input.");
			else if (string.IsNullOrWhiteSpace(StartTimeInput.Text)
				&& string.IsNullOrWhiteSpace(EndTimeInput.Text)
				&& EventsPage.DataManager.EventList.Any(x => x.StartTime == null && x.Date == DateOnly.Parse(DateInput.Text)
				&& x != @event))
				errorList.Add("Cannot have multiple all-day events on the same day.");

			if (!string.IsNullOrWhiteSpace(StartTimeInput.Text) && !TimeOnly.TryParse(StartTimeInput.Text, out _))
				errorList.Add("Invalid start time input.");

			if (!string.IsNullOrWhiteSpace(EndTimeInput.Text) && !TimeOnly.TryParse(EndTimeInput.Text, out _))
				errorList.Add("Invalid end time input.");

			if (TimeOnly.TryParse(StartTimeInput.Text, out TimeOnly startTime) && TimeOnly.TryParse(EndTimeInput.Text, out TimeOnly endTime))
			{
				if (startTime.CompareTo(endTime) >= 0)
					errorList.Add("End time must be after start time.");
				else if ((endTime - startTime).TotalMinutes < 15)
					errorList.Add("Event must be at least 15 minutes.");

				if (DateOnly.TryParse(DateInput.Text, out DateOnly date)
					&& EventsPage.DataManager.EventList
					.Any(x => x != @event && x.Date == date && x.StartTime < endTime && x.EndTime > startTime))
					errorList.Add("Times must not overlap with existing events.");
			}

			if (errorList.Count != 0)
			{
				MessageBox.Show(string.Join('\n', errorList), "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				EventsPage.EventEditorPopup.IsOpen = true;
			}

			return errorList.Count == 0;
		}
		private void CancelEdit() => EventsPage.EventEditorPopup.IsOpen = false;
		private void SaveEvent()
		{
			if (CheckInputs())
			{
				@event.SetValues(TitleInput.Text, DetailsInput.Text, LocationInput.Text, DateInput.Text, StartTimeInput.Text, EndTimeInput.Text, selectedColour);

				if (newEvent)
					EventsPage.DataManager.EventList.Add(@event);

				EventsPage.UpdateEventList();
				EventsPage.EventEditorPopup.IsOpen = false;
			}
		}
		private void CancelButtonClick(object sender, RoutedEventArgs e) => CancelEdit();
		private void SaveButtonClick(object sender, RoutedEventArgs e) => SaveEvent();
	}
}