using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

public partial class SilentWolf : Node {
	private Node silentWolfGD;

	public Node Scores { get; private set; }

	public StringName version {
		get { return silentWolfGD.Get("version").AsString(); }
	}

	public StringName godot_version {
		get { return silentWolfGD.Get("godot_version").AsString(); }
		set { silentWolfGD.Set("godot_version", value); }
	}

	// 	silentWolfScores.Call("save_score", settings.settingsData.PlayerName, Score);

	public override void _Ready () {

		silentWolfGD = GetNode<Node>("/root/SilentWolf");
		Scores = (Node)silentWolfGD.Get("Scores");
	}

	public void ConfigureAPIKey (string api_key) => silentWolfGD.Call("configure_api_key", api_key);
	public void ConfigureGameId (string game_id) => silentWolfGD.Call("configure_game_id", game_id);
	public void ConfigureGameVersion (string game_version) => silentWolfGD.Call("configure_game_version", game_version);


}

