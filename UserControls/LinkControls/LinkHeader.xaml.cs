using System.Windows;
using System.Windows.Controls;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.LinkControls
{
	public partial class LinkHeader : UserControl
	{
		public required LinksPage LinksPage { get; init; }
		public string GroupText { get; init; } = string.Empty;
		public string SubgroupText { get; init; } = string.Empty;

		private readonly Style titleStyle = (Style)Application.Current.FindResource("Title");
		private readonly Style headerCollapseButtonSmallStyle = (Style)Application.Current.FindResource("HeaderCollapseButtonSmall");
		private readonly Style headerExpandButtonSmallStyle = (Style)Application.Current.FindResource("HeaderExpandButtonSmall");

		public LinkHeader() => InitializeComponent();
		public LinkHeader SetDisplay()
		{
			if (!string.IsNullOrEmpty(SubgroupText))
			{
				HeaderText.Text = SubgroupText;
				HeaderText.Style = titleStyle;
				Margin = new Thickness(40, 0, 0, 0);
				CollapseButton.Style = headerCollapseButtonSmallStyle;
				ExpandButton.Style = headerExpandButtonSmallStyle;
			}
			else
				HeaderText.Text = GroupText;

			CollapseButton.Visibility = Visibility.Visible;
			return this;
		}

		public void CollapseGroup()
		{
			CollapseButton.Visibility = Visibility.Collapsed;
			ExpandButton.Visibility = Visibility.Visible;

			if (string.IsNullOrEmpty(SubgroupText))
				LinksPage.CollapseGroup(GroupText);
			else
				LinksPage.CollapseSubGroup(GroupText, SubgroupText);
		}
		public void ExpandGroup()
		{
			CollapseButton.Visibility = Visibility.Visible;
			ExpandButton.Visibility = Visibility.Collapsed;

			if (string.IsNullOrEmpty(SubgroupText))
				LinksPage.ExpandGroup(GroupText);
			else
				LinksPage.ExpandSubGroup(GroupText, SubgroupText);
		}
		private void CollapseButtonClick(object sender, RoutedEventArgs e) => CollapseGroup();
		private void ExpandButtonClick(object sende, RoutedEventArgs e) => ExpandGroup();
	}
}