using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class CreditScene : Control {
	private SceneSwitcher sceneSwitcher;
	public Label Designer;
	public Label Programmer;
	public Label MusicArtist;
	public Label SFXArtist;
	public Label ArtDesigner;
	public Label FontCredit;

	public override void _Ready () {
		sceneSwitcher = GetNodeOrNull<SceneSwitcher>("/root/SceneSwitcher");
	}

	public override void _Input (InputEvent inputEvent) {
		if (Input.IsActionJustPressed("Back")) {
			BackToMainMenu();
		}
	}

	public void BackToMainMenu () {
		sceneSwitcher?.GotoScene("res://Escenas/MainMenu.tscn");

	}
}

