using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.LinkControls
{
	public partial class LinkViewer : UserControl
	{
		public required LinksPage LinksPage { get; init; }
		public required Link Link { get; init; }

		public LinkViewer() => InitializeComponent();
		public LinkViewer SetDisplay()
		{
			LinkText.Text = Link.Title;

			if (Link.Subgroup != null)
				Margin = new Thickness(80, 0, 0, 0);
			else if (Link.Group != null)
				Margin = new Thickness(40, 0, 0, 0);

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
						FileName = LinksPage.DataManager.Settings.Browser,
						Arguments = LinksPage.DataManager.Settings.Arguments() + Link.Url
					});
				}
				catch
				{
					MessageBox.Show($"Could not start program \"{LinksPage.DataManager.Settings.Browser}\".", "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
				MessageBox.Show("Could not open url.", "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		private void EditLink()
		{
			((LinkEditor)LinksPage.LinkEditorPopup.Child).SetDisplay(Link);
			LinksPage.LinkEditorPopup.IsOpen = true;
			((LinkEditor)LinksPage.LinkEditorPopup.Child).FocusKeyboard();
		}
		private void DeleteLink()
		{
			LinksPage.DataManager.LinkList.Remove(Link);
			LinksPage.UpdateLinkList();
		}
		private void LinkButtonClick(object sender, MouseButtonEventArgs e) => OpenLink();
		private void EditButtonClick(object sender, RoutedEventArgs e) => EditLink();
		private void DeleteButtonClick(object sender, RoutedEventArgs e) => DeleteLink();

		private void ShowButtons()
		{
			EditButton.Visibility = Visibility.Visible;
			DeleteButton.Visibility = Visibility.Visible;
			LinkText.TextDecorations = TextDecorations.Underline;
		}
		private void HideButtons()
		{
			EditButton.Visibility = Visibility.Collapsed;
			DeleteButton.Visibility = Visibility.Collapsed;
			LinkText.TextDecorations = null;
		}
		private void ShowButtonsEvent(object sender, MouseEventArgs e) => ShowButtons();
		private void HideButtonsEvent(object sender, MouseEventArgs e) => HideButtons();
	}
}