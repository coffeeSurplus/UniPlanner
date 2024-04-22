using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;

namespace UniPlanner.UserControls.HomeControls
{
	public partial class HomePageLinkViewer : UserControl
	{
		public required DataManager DataManager { get; init; }
		public required Link Link { get; init; }

		public HomePageLinkViewer() => InitializeComponent();
		public HomePageLinkViewer SetDisplay()
		{
			LinkText.Text = Link.Title;
			return this;
		}

		private void OpenLink()
		{
			if (Directory.Exists(Link.Url))
				Process.Start("explorer.exe", Link.Url);
			else if (Uri.IsWellFormedUriString(Link.Url, UriKind.Absolute))
			{
				try
				{
					Process.Start(new ProcessStartInfo()
					{
						UseShellExecute = true,
						FileName = DataManager.Settings.Browser,
						Arguments = DataManager.Settings.Arguments() + Link.Url
					});
				}
				catch
				{
					MessageBox.Show($"Could not start program \"{DataManager.Settings.Browser}\".", "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
				MessageBox.Show("Could not open url.", "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		private void LinkButtonClick(object sender, MouseButtonEventArgs e) => OpenLink();

		private void ShowButtons() => LinkText.TextDecorations = TextDecorations.Underline;
		private void HideButtons() => LinkText.TextDecorations = null;
		private void ShowButtonsEvent(object sender, MouseEventArgs e) => ShowButtons();
		private void HideButtonsEvent(object sender, MouseEventArgs e) => HideButtons();
	}
}
