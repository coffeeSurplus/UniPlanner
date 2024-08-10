using System.IO;
using System.Text.Json;

namespace UniPlanner.Source.Data;

internal class DataManager<T>
{
	private string Path { get; }
	public T Data { get; }

	public DataManager(string path)
	{
		Path = $"{path}.json";
		Data = File.Exists(GetPath()) ? JsonSerializer.Deserialize<T>(File.ReadAllText(GetPath()))! : (T)Activator.CreateInstance(typeof(T))!;
	}

	public void UpdateData() => File.WriteAllText(GetPath(), JsonSerializer.Serialize(Data));

	private string GetPath() => Environment.ExpandEnvironmentVariables($@"%AppData%\UniPlanner\{Path}");
}