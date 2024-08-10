using System.Diagnostics;
using System.IO;
using System.Windows;

namespace UniPlanner.Source.Models;

internal class LinkModel
{
	public string Title { get; set; } = string.Empty;
	public string? Group { get; set; }
	public string? Subgroup { get; set; }
	public string URL { get; set; } = string.Empty;
	public bool Favourite { get; set; }

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
				MessageBox.Show($"Could not start program \"{browser}\".", "UniPlanner");
			}
		}
	}
}