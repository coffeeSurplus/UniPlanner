using Microsoft.Win32;
using System.IO;
using System.Windows;
using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class LinkViewModel : ViewModelBase
{
	private readonly DataManager<List<LinkModel>> dataManager = MainProgram.LinkManager;
	private readonly SettingsModel settings = MainProgram.SettingsManager.Data;
	private readonly List<LinkModel> linkList = MainProgram.LinkManager.Data;
	private bool newLink = false;
	private LinkModel currentLink = new();

	private bool expanderBindingEnabled = true;
	private bool expanded = true;
	private bool showAll = true;
	private bool linkEditorVisible = false;
	private bool defaultMessageVisible = false;
	private string currentLinkTitle = string.Empty;
	private string currentLinkGroup = string.Empty;
	private string currentLinkSubgroup = string.Empty;
	private string currentLinkURL = string.Empty;
	private bool currentLinkFavourite = false;

	public bool ExpanderBindingEnabled
	{
		get => expanderBindingEnabled;
		set => SetValue(ref expanderBindingEnabled, value);
	}
	public bool Expanded
	{
		get => expanded;
		set => SetValue(ref expanded, value);
	}
	public bool ShowAll
	{
		get => showAll;
		set => SetValue(ref showAll, value);
	}
	public bool LinkEditorVisible
	{
		get => linkEditorVisible;
		set => SetValue(ref linkEditorVisible, value);
	}
	public bool DefaultMessageVisible
	{
		get => defaultMessageVisible;
		set => SetValue(ref defaultMessageVisible, value);
	}
	public string CurrentLinkTitle
	{
		get => currentLinkTitle;
		set => SetValue(ref currentLinkTitle, value);
	}
	public string CurrentLinkGroup
	{
		get => currentLinkGroup;
		set => SetValue(ref currentLinkGroup, value);
	}
	public string CurrentLinkSubgroup
	{
		get => currentLinkSubgroup;
		set => SetValue(ref currentLinkSubgroup, value);
	}
	public string CurrentLinkURL
	{
		get => currentLinkURL;
		set => SetValue(ref currentLinkURL, value);
	}
	public bool CurrentLinkFavourite
	{
		get => currentLinkFavourite;
		set => SetValue(ref currentLinkFavourite, value);
	}

	public RelayCommand EnableExpanderBindingCommand { get; }
	public RelayCommand UpdateViewCommand { get; }
	public RelayCommand NewLinkCommand { get; }
	public RelayCommand OpenLinkCommand { get; }
	public RelayCommand EditLinkCommand { get; }
	public RelayCommand RemoveLinkCommand { get; }
	public RelayCommand SelectFolderCommand { get; }
	public RelayCommand CancelEditLinkCommand { get; }
	public RelayCommand SaveEditLinkCommand { get; }

	public LinkCollectionView LinkCollectionView { get; } = new(MainProgram.LinkManager.Data);

	public LinkViewModel()
	{
		EnableExpanderBindingCommand = new(EnableExpanderBinding);
		UpdateViewCommand = new(UpdateView);
		NewLinkCommand = new(NewLink);
		OpenLinkCommand = new(OpenLink);
		EditLinkCommand = new(EditLink);
		RemoveLinkCommand = new(RemoveLink);
		SelectFolderCommand = new(SelectFolder);
		CancelEditLinkCommand = new(CancelEditLink);
		SaveEditLinkCommand = new(SaveEditLink);
		UpdateView(false);
	}

	private void EnableExpanderBinding(object parameter) => ExpanderBindingEnabled = true;
	private void UpdateView(object parameter) => UpdateView();
	private void NewLink(object parameter)
	{
		newLink = true;
		currentLink = new();
		CurrentLinkTitle = string.Empty;
		CurrentLinkGroup = string.Empty;
		CurrentLinkSubgroup = string.Empty;
		CurrentLinkURL = string.Empty;
		CurrentLinkFavourite = false;
		LinkEditorVisible = true;
	}
	private void OpenLink(object parameter) => LinkModel.OpenUrl((string)parameter, settings.ReturnBrowser(), settings.ReturnArguments());
	private void EditLink(object parameter)
	{
		newLink = false;
		currentLink = (LinkModel)parameter;
		CurrentLinkTitle = currentLink.Title;
		CurrentLinkGroup = currentLink.Group ?? string.Empty;
		CurrentLinkSubgroup = currentLink.Subgroup ?? string.Empty;
		CurrentLinkURL = currentLink.URL;
		CurrentLinkFavourite = currentLink.Favourite;
		LinkEditorVisible = true;
	}
	private void RemoveLink(object parameter)
	{
		linkList.Remove((LinkModel)parameter);
		UpdateData();
		UpdateView();
	}
	private void SelectFolder(object parameter)
	{
		OpenFolderDialog folderDialog = new();
		bool folderSelected = folderDialog.ShowDialog() == true;
		LinkEditorVisible = true;
		if (folderSelected)
		{
			CurrentLinkURL = folderDialog.FolderName;
		}
	}
	private void CancelEditLink(object parameter) => LinkEditorVisible = false;
	private void SaveEditLink(object parameter)
	{
		if (CheckLinkValues())
		{
			currentLink.Title = CurrentLinkTitle.Trim().ToLower();
			currentLink.Group = !string.IsNullOrWhiteSpace(CurrentLinkGroup) ? CurrentLinkGroup.Trim().ToLower() : null;
			currentLink.Subgroup = !string.IsNullOrWhiteSpace(CurrentLinkGroup) && !string.IsNullOrWhiteSpace(CurrentLinkSubgroup) ? currentLinkSubgroup.Trim().ToLower() : null;
			currentLink.URL = LinkModel.FormUrl(CurrentLinkURL.Trim());
			currentLink.Favourite = CurrentLinkFavourite;
			if (newLink)
			{
				linkList.Add(currentLink);
			}
			LinkEditorVisible = false;
			UpdateData();
			UpdateView();
		}
	}

	private bool CheckLinkValues()
	{
		if (!string.IsNullOrWhiteSpace(CurrentLinkTitle))
		{
			if (!string.IsNullOrWhiteSpace(CurrentLinkURL) && (Directory.Exists(CurrentLinkURL.Trim()) || Uri.IsWellFormedUriString(LinkModel.FormUrl(CurrentLinkURL.Trim()), UriKind.Absolute)))
			{
				return true;
			}
			else
			{
				MessageBox.Show("Invalid URL.", "UniPlanner");
			}
		}
		else
		{
			MessageBox.Show("Invalid title.", "UniPlanner");
		}
		LinkEditorVisible = true;
		return false;
	}
	private void UpdateData() => dataManager.UpdateData();
	private void UpdateView(bool updateHomeView = true)
	{
		LinkCollectionView.UpdateView(ShowAll);
		DefaultMessageVisible = linkList.Count == 0 || !ShowAll && linkList.All(x => !x.Favourite);
		if (updateHomeView)
		{
			MainProgram.UpdateHomeView();
		}
	}
}