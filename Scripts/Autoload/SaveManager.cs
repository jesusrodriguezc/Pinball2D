using Godot;
using Newtonsoft.Json;
/// <summary>
/// Class that can load and save files with user information
/// </summary>
public partial class SaveManager : Node {

	public bool Load<T> (string loadingPath, out T data) {
		data = default(T);
		if (!FileAccess.FileExists(loadingPath)) {
			return false;
		}
		using var file = FileAccess.Open(loadingPath, FileAccess.ModeFlags.Read);
		string jsonContent = file.GetAsText();
		data = JsonConvert.DeserializeObject<T>(jsonContent);
		return true;
	}
	
	public void Save<T> (T data, string savingPath) {
		using var file = FileAccess.Open(savingPath, FileAccess.ModeFlags.Write);
		string jsonContent = JsonConvert.SerializeObject(data);
		file.StoreString(jsonContent);
	}
}

