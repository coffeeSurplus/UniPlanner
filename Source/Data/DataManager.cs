using System.IO;
using System.Text.Json;

namespace UniPlanner.Source.Data;

internal class DataManager<T>
{
	private readonly string path;

	public T Data { get; }

	public DataManager(string path)
	{
		this.path = $"{path}.json";
		Data = File.Exists(GetPath()) ? JsonSerializer.Deserialize<T>(File.ReadAllText(GetPath()))! : Activator.CreateInstance<T>();
	}

	public void UpdateData() => File.WriteAllText(GetPath(), JsonSerializer.Serialize(Data));

	private string GetPath() => Environment.ExpandEnvironmentVariables(@$"%AppData%\UniPlanner\{path}");
}