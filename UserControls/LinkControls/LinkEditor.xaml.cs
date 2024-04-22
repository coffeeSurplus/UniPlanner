using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.LinkControls
{
	public partial class LinkEditor : UserControl
	{
		public required LinksPage LinksPage { get; init; }

		private readonly OpenFolderDialog folderDialog = new();
		private Link Link = new() { Title = string.Empty, Url = string.Empty, Favourite = false };
		private bool favourite = false;
		private bool newLink = false;

		public LinkEditor() => InitializeComponent();

		public void SetDefaultDisplay()
		{
			Link = new() { Title = string.Empty, Url = string.Empty, Favourite = false };
			TitleInput.Text = string.Empty;
			GroupInput.Text = null;
			SubgroupInput.Text = null;
			UrlInput.Text = string.Empty;
			favourite = false;
			newLink = true;
			SetFavouriteButton();
		}
		public void SetDisplay(Link link)
		{
			Link = link;
			TitleInput.Text = link.Title;
			GroupInput.Text = link.Group;
			SubgroupInput.Text = link.Subgroup;
			UrlInput.Text = link.Url;
			favourite = link.Favourite;
			newLink = false;
			SetFavouriteButton();
		}
		public void FocusKeyboard() => Keyboard.Focus(TitleInput);

		private void SetFavouriteButton() => FavouriteButton.Style = (Style)FindResource(favourite ? "SelectedButton" : "DeselectedButton");
		private void ToggleFavourite()
		{
			favourite ^= true;
			SetFavouriteButton();
		}
		private void SelectFolder()
		{
			bool folderSelected = folderDialog.ShowDialog() == true;
			LinksPage.LinkEditorPopup.IsOpen = true;
			if (folderSelected)
				UrlInput.Text = folderDialog.FolderName;
		}
		private void FavouriteButtonClick(object sender, RoutedEventArgs e) => ToggleFavourite();
		private void SelectFolderButtonClick(object sender, RoutedEventArgs e) => SelectFolder();

		private bool CheckInputs()
		{
			List<string> errorList = [];

			if (string.IsNullOrWhiteSpace(TitleInput.Text))
				errorList.Add("Link title must not be empty.");

			if (string.IsNullOrWhiteSpace(UrlInput.Text))
				errorList.Add("Link url must not be empty.");
			else if (!(Directory.Exists(UrlInput.Text.Trim())
				|| Uri.IsWellFormedUriString(Link.FormUrl(UrlInput.Text.Trim()), UriKind.Absolute)))
				errorList.Add("Invalid url.");

			if (errorList.Count != 0)
			{
				MessageBox.Show(string.Join('\n', errorList), "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				LinksPage.LinkEditorPopup.IsOpen = true;
			}

			return errorList.Count == 0;
		}
		private void CancelEdit() => LinksPage.LinkEditorPopup.IsOpen = false;
		private void SaveLink()
		{
			if (CheckInputs())
			{
				Link.SetValues(TitleInput.Text, GroupInput.Text, SubgroupInput.Text, UrlInput.Text, favourite);

				if (newLink)
					LinksPage.DataManager.LinkList.Add(Link);

				LinksPage.UpdateLinkList();
				LinksPage.LinkEditorPopup.IsOpen = false;
			}
		}
		private void CancelButtonClick(object sender, RoutedEventArgs e) => CancelEdit();
		private void SaveButtonClick(object sender, RoutedEventArgs e) => SaveLink();
	}
}