using System.Diagnostics;
using System.IO;
using UniPlanner.Source.Data;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.Models;

internal class LinkModel : ViewModelBase
{
	private string title = string.Empty;
	private string? group = null;
	private string? subgroup = null;
	private string uRL = string.Empty;
	private bool favourite = false;

	public string Title
	{
		get => title;
		set => SetValue(ref title, value);
	}
	public string? Group
	{
		get => group;
		set => SetValue(ref group, value);
	}
	public string? Subgroup
	{
		get => subgroup;
		set => SetValue(ref subgroup, value);
	}
	public string URL
	{
		get => uRL;
		set => SetValue(ref uRL, value);
	}
	public bool Favourite
	{
		get => favourite;
		set => SetValue(ref favourite, value);
	}

	public static string FormUrl(string url) => (Directory.Exists(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute)) ? url : url.EndsWith('.') ? $"https://{url}com" : url.Contains('.') ? $"https://{url}" : $"https://{url}.com";
	public static void OpenUrl(string url, string browser, string arguments)
	{
		if (Directory.Exists(url))
		{
			Process.Start((ProcessStartInfo)new("explorer.exe", url));
		}
		else
		{
			try
			{
				Process.Start(new ProcessStartInfo() { FileName = browser, Arguments = arguments + url, UseShellExecute = true });
			}
			catch
			{
				Popup.MessageBox($"Could not start program \"{browser}\".");
			}
		}
	}
}