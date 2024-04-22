using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TimetableControls
{
	public partial class TimetableEditor : UserControl
	{
		public required TimetablePage TimetablePage { get; init; }

		private readonly Style SelectedColourButtonStyle = (Style)Application.Current.FindResource("SelectedColourButton");
		private readonly Style DeselectedColourButtonStyle = (Style)Application.Current.FindResource("DeselectedColourButton");
		private readonly Style SelectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style DeselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");

		private Timetable timetable = new() { Title = string.Empty, Colour = 0, Day = new(), StartTime = new(), EndTime = new() };
		private int selectedDay = 1;
		private int selectedColour = 0;
		private bool newTimetable = false;

		public TimetableEditor() => InitializeComponent();
		public void SetDefaultDisplay()
		{
			timetable = new() { Title = string.Empty, Colour = 0, Day = new(), StartTime = new(), EndTime = new() };
			TitleInput.Text = string.Empty;
			DetailsInput.Text = string.Empty;
			SubjectInput.Text = string.Empty;
			LocationInput.Text = string.Empty;
			selectedDay = 1;
			StartTimeInput.Text = string.Empty;
			EndTimeInput.Text = string.Empty;
			selectedColour = 0;
			newTimetable = true;
			SetDayButtons(MondayButton);
			SetColourButtons(WhiteButton);
		}
		public void SetDisplay(Timetable timetable)
		{
			this.timetable = timetable;
			TitleInput.Text = timetable.Title;
			DetailsInput.Text = timetable.Details;
			SubjectInput.Text = timetable.Subject;
			LocationInput.Text = timetable.Location;
			selectedDay = timetable.Day;
			StartTimeInput.Text = $"{timetable.StartTime:H:mm}";
			EndTimeInput.Text = $"{timetable.EndTime:H:mm}";
			selectedColour = timetable.Colour;
			newTimetable = false;

			SetDayButtons(timetable.Day switch
			{
				1 => MondayButton,
				2 => TuesdayButton,
				3 => WednesdayButton,
				4 => ThursdayButton,
				_ => FridayButton
			});

			SetColourButtons(timetable.Colour switch
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

		private void SetDayButtons(Button selectedButton)
		{
			foreach (Button button in DayButtons.Children)
				button.Style = DeselectedButtonStyle;

			selectedButton.Style = SelectedButtonStyle;

			selectedDay = selectedButton.Name switch
			{
				"MondayButton" => 1,
				"TuesdayButton" => 2,
				"WednesdayButton" => 3,
				"ThursdayButton" => 4,
				_ => 5
			};
		}
		private void DayButtonClick(object sender, RoutedEventArgs e) => SetDayButtons((Button)sender);

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

		private void CancelEdit() => TimetablePage.TimetableEditorPopup.IsOpen = false;
		private bool CheckInputs()
		{
			List<string> errorList = [];

			if (string.IsNullOrWhiteSpace(TitleInput.Text))
				errorList.Add("Title must not be empty.");

			if (string.IsNullOrWhiteSpace(StartTimeInput.Text) || !TimeOnly.TryParse(StartTimeInput.Text, out _))
				errorList.Add("Invalid start time input.");

			if (string.IsNullOrWhiteSpace(EndTimeInput.Text) || !TimeOnly.TryParse(EndTimeInput.Text, out _))
				errorList.Add("Invalid end time input.");

			if (TimeOnly.TryParse(StartTimeInput.Text, out TimeOnly startTime) && TimeOnly.TryParse(EndTimeInput.Text, out TimeOnly endTime))
			{
				if (startTime.CompareTo(endTime) >= 0)
					errorList.Add("End time must be after start time.");
				else if ((endTime - startTime).TotalMinutes < 15)
					errorList.Add("Event must be at least 15 minutes.");

				if (TimetablePage.DataManager.TimetableList.Any(x => x != timetable
					&& x.Day == selectedDay
					&& x.StartTime < endTime
					&& x.EndTime > startTime))
					errorList.Add("Times must not overlap with existing events.");
			}

			if (errorList.Count != 0)
			{
				MessageBox.Show(string.Join('\n', errorList), "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				TimetablePage.TimetableEditorPopup.IsOpen = true;
			}

			return errorList.Count == 0;
		}
		private void SaveTimetable()
		{
			if (CheckInputs())
			{
				timetable.SetValues(TitleInput.Text, DetailsInput.Text, SubjectInput.Text, LocationInput.Text, selectedDay, StartTimeInput.Text, EndTimeInput.Text, selectedColour);

				if (newTimetable)
					TimetablePage.DataManager.TimetableList.Add(timetable);

				TimetablePage.UpdateTimetableList();
				TimetablePage.TimetableEditorPopup.IsOpen = false;
			}
		}
		private void CancelButtonClick(object sender, RoutedEventArgs e) => CancelEdit();
		private void SaveButtonClick(object sender, RoutedEventArgs e) => SaveTimetable();
	}
}