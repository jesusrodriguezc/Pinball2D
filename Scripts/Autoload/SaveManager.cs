using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
		data = JsonSerializer.Deserialize<T>(jsonContent);
		return true;
	}
	
	public void Save<T> (T data, string savingPath) {
		using var file = FileAccess.Open(savingPath, FileAccess.ModeFlags.Write);
		string jsonContent = JsonSerializer.Serialize(data);
		file.StoreString(jsonContent);
	}
}

