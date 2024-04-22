using System.Windows;
using System.Windows.Controls;
using UniPlanner.Classes;
using UniPlanner.UserControls.LinkControls;

namespace UniPlanner.Pages
{
	public partial class LinksPage : Page
	{
		public required DataManager DataManager { get; init; }

		private readonly Style selectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style deselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");

		private List<Link> linkList = [];
		private bool showAll = true;

		public LinksPage() => InitializeComponent();
		public LinksPage SetDisplay()
		{
			UpdateView();
			LinkEditorPopup.Child = new LinkEditor() { LinksPage = this };
			return this;
		}

		public void CollapseGroup(string header)
		{
			foreach (var obj in LinkArea.Children)
			{
				if (obj is LinkViewer linkViewer && linkViewer.Link.Group == header)
					linkViewer.Visibility = Visibility.Collapsed;
				else if (obj is LinkHeader linkHeader && linkHeader.GroupText == header
					&& linkHeader.CollapseButton.Visibility == Visibility.Visible)
					linkHeader.CollapseGroup();
			}

			SetViewButtons();
		}
		public void ExpandGroup(string header)
		{
			foreach (var obj in LinkArea.Children)
			{
				if (obj is LinkViewer linkViewer && linkViewer.Link.Group == header)
					linkViewer.Visibility = Visibility.Visible;
				else if (obj is LinkHeader linkHeader && linkHeader.GroupText == header
					&& linkHeader.CollapseButton.Visibility == Visibility.Collapsed)
					linkHeader.ExpandGroup();
			}

			SetViewButtons();
		}
		public void CollapseSubGroup(string header, string subheader)
		{
			foreach (var obj in LinkArea.Children)
				if (obj is LinkViewer linkViewer && linkViewer.Link.Group == header
					&& linkViewer.Link.Subgroup == subheader)
					linkViewer.Visibility = Visibility.Collapsed;

			SetViewButtons();
		}
		public void ExpandSubGroup(string header, string subheader)
		{
			foreach (var obj in LinkArea.Children)
				if (obj is LinkViewer linkViewer && linkViewer.Link.Group == header
					&& linkViewer.Link.Subgroup == subheader)
					linkViewer.Visibility = Visibility.Visible;

			SetViewButtons();
		}
		public void UpdateLinkList()
		{
			DataManager.UpdateLinkList();
			((MainWindow)Application.Current.MainWindow).UpdateDisplay();
		}

		private void UpdateView()
		{
			linkList = [.. DataManager.LinkList
				.OrderBy(x => x.Group == null)
				.ThenBy(x => x.Group)
				.ThenBy(x => x.Subgroup)
				.ThenBy(x => x.Title)];

			LinkArea.Children.RemoveRange(0, LinkArea.Children.Count);
			AddDefaultHeader();

			for (int index = 0; index < linkList.Count; index++)
			{
				AddHeader(index);
				AddSubHeader(index);
				AddLink(index);
			}

			SetViewButtons();
		}
		private void AddDefaultHeader()
		{
			if (showAll ? linkList.Count == 0 : !linkList.Any(x => x.Favourite))
				LinkArea.Children.Add(new LinkHeader() { LinksPage = this });
		}
		private void AddHeader(int index)
		{
			string? header = linkList[index].Group;

			if (header != null
				&& (index == 0 || header != linkList[index - 1].Group)
				&& (showAll || linkList.Any(x => x.Group == header && x.Favourite)))
				LinkArea.Children.Add(new LinkHeader() { LinksPage = this, GroupText = header }.SetDisplay());
		}
		private void AddSubHeader(int index)
		{
			string? header = linkList[index].Group;
			string? subheader = linkList[index].Subgroup;

			if (header != null
				&& subheader != null
				&& (index == 0 || subheader != linkList[index - 1].Subgroup)
				&& (showAll || linkList.Any(x => x.Subgroup == subheader && x.Favourite)))
				LinkArea.Children.Add(new LinkHeader() { LinksPage = this, GroupText = header, SubgroupText = subheader }.SetDisplay());
		}
		private void AddLink(int index)
		{
			if (showAll || linkList[index].Favourite)
				LinkArea.Children.Add(new LinkViewer() { LinksPage = this, Link = linkList[index] }.SetDisplay());
		}

		private void SetViewButtons()
		{
			bool expanded = showAll ? linkList.Count != 0 : linkList.Any(x => x.Favourite);
			bool collapsed = showAll ? linkList.Count != 0 : linkList.Any(x => x.Favourite);

			foreach (var obj in LinkArea.Children)
			{
				if (obj is LinkHeader linkHeader)
				{
					if (linkHeader.ExpandButton.Visibility == Visibility.Visible)
						expanded = false;
					else if (linkHeader.CollapseButton.Visibility == Visibility.Visible)
						collapsed = false;
				}
			}

			ExpandButton.Style = expanded ? selectedButtonStyle : deselectedButtonStyle;
			CollapseButton.Style = collapsed ? selectedButtonStyle : deselectedButtonStyle;
		}
		private void ExpandAll()
		{
			foreach (var obj in LinkArea.Children)
				if (obj is LinkHeader linkHeader)
					linkHeader.ExpandGroup();

			SetViewButtons();
		}
		private void CollapseAll()
		{
			foreach (var obj in LinkArea.Children)
				if (obj is LinkHeader linkHeader)
					linkHeader.CollapseGroup();

			SetViewButtons();
		}
		private void ExpandAllButtonClick(object sender, RoutedEventArgs e) => ExpandAll();
		private void CollapseAllButtonClick(object sender, RoutedEventArgs e) => CollapseAll();

		private void ShowAll()
		{
			ShowAllButton.Style = selectedButtonStyle;
			FavouritesOnlyButton.Style = deselectedButtonStyle;
			showAll = true;
			UpdateView();
		}
		private void FavouritesOnly()
		{
			ShowAllButton.Style = deselectedButtonStyle;
			FavouritesOnlyButton.Style = selectedButtonStyle;
			showAll = false;
			UpdateView();
		}
		private void ShowAllButtonClick(object sender, RoutedEventArgs e) => ShowAll();
		private void FavouritesOnlyButtonClick(object sender, RoutedEventArgs e) => FavouritesOnly();

		private void NewLink()
		{
			((LinkEditor)LinkEditorPopup.Child).SetDefaultDisplay();
			LinkEditorPopup.IsOpen = true;
			((LinkEditor)LinkEditorPopup.Child).FocusKeyboard();
		}
		private void NewLinkButtonClick(object sender, RoutedEventArgs e) => NewLink();
	}
}