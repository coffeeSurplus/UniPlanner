using Microsoft.Win32;
using System.IO;
using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class LinkViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;
	private readonly List<LinkModel> linkList;
	private bool newLink = true;
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
	private bool currentLinkFavourite;

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
	public RelayCommand SelectFolderCommand { get; }
	public RelayCommand CancelEditLinkCommand { get; }
	public RelayCommand SaveEditLinkCommand { get; }
	public RelayCommand<string> OpenLinkCommand { get; }
	public RelayCommand<LinkModel> EditLinkCommand { get; }
	public RelayCommand<LinkModel> RemoveLinkCommand { get; }

	public LinkCollectionView LinkCollectionView { get; }

	public LinkViewModel(DataAccess data)
	{
		dataAccess = data;
		linkList = dataAccess.LinkList;
		LinkCollectionView = new(linkList);
		EnableExpanderBindingCommand = new(EnableExpanderBinding);
		UpdateViewCommand = new(UpdateView);
		NewLinkCommand = new(NewLink);
		SelectFolderCommand = new(SelectFolder);
		CancelEditLinkCommand = new(CancelEditLink);
		SaveEditLinkCommand = new(SaveEditLink);
		OpenLinkCommand = new(OpenLink);
		EditLinkCommand = new(EditLink);
		RemoveLinkCommand = new(RemoveLink);
		UpdateView(false);
	}

	private void EnableExpanderBinding() => ExpanderBindingEnabled = true;
	private void UpdateView() => UpdateView(true);
	private void NewLink()
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
	private void SelectFolder()
	{
		OpenFolderDialog folderDialog = new();
		bool folderSelected = folderDialog.ShowDialog() == true;
		LinkEditorVisible = true;
		if (folderSelected)
		{
			CurrentLinkURL = folderDialog.FolderName;
		}
	}
	private void CancelEditLink() => LinkEditorVisible = false;
	private void SaveEditLink()
	{
		if (CheckLinkValues())
		{
			currentLink.Title = CurrentLinkTitle.Trim().ToLower();
			currentLink.Group = !string.IsNullOrWhiteSpace(CurrentLinkGroup) ? CurrentLinkGroup.Trim().ToLower() : null;
			currentLink.Subgroup = !string.IsNullOrWhiteSpace(CurrentLinkGroup) && !string.IsNullOrWhiteSpace(CurrentLinkSubgroup) ? CurrentLinkSubgroup.Trim().ToLower() : null;
			currentLink.URL = LinkModel.FormUrl(CurrentLinkURL.Trim());
			currentLink.Favourite = CurrentLinkFavourite;
			if (newLink)
			{
				linkList.Add(currentLink);
			}
			LinkEditorVisible = false;
			UpdateData();
			UpdateView(true);
		}
	}
	private void OpenLink(string parameter) => LinkModel.OpenUrl(parameter, dataAccess.Settings.ReturnBrowser(), dataAccess.Settings.ReturnArguments());
	private void EditLink(LinkModel parameter)
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
	private void RemoveLink(LinkModel parameter)
	{
		linkList.Remove((LinkModel)parameter);
		UpdateData();
		UpdateView(true);
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
				Popup.MessageBox("Invalid URL.");
			}
		}
		else
		{
			Popup.MessageBox("Invalid title.");
		}
		LinkEditorVisible = true;
		return false;
	}
	private void UpdateData() => dataAccess.UpdateLinkList();
	private void UpdateView(bool updateHomeView)
	{
		LinkCollectionView.UpdateView(ShowAll);
		DefaultMessageVisible = linkList.Count == 0 || !ShowAll && linkList.All(x => !x.Favourite);
		if (updateHomeView)
		{
			DataAccess.MainWindow.UpdateHomeView();
		}
	}
}