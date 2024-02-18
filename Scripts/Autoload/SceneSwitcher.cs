using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class SceneSwitcher : Node {
	private AnimationPlayer animationPlayer;

	public Node CurrentScene { get; set; }

	public override void _Ready () {
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Viewport root = GetTree().Root;
		CurrentScene = root.GetChild(root.GetChildCount() - 1);
	}

	public void GotoScene (string path) {
		// This function will usually be called from a signal callback,
		// or some other function from the current scene.
		// Deleting the current scene at this point is
		// a bad idea, because it may still be executing code.
		// This will result in a crash or unexpected behavior.

		// The solution is to defer the load to a later time, when
		// we can be sure that no code from the current scene is running:

		CallDeferred(MethodName.DeferredGotoScene2, path);
	}

	public void DeferredGotoScene (string path) {
		// It is now safe to remove the current scene.
		CurrentScene.Free();

		// Load a new scene.
		var nextScene = GD.Load<PackedScene>(path);

		// Instance the new scene.
		CurrentScene = nextScene.Instantiate();

		// Add it to the active scene, as child of root.
		GetTree().Root.AddChild(CurrentScene);

		// Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
		GetTree().CurrentScene = CurrentScene;
	}

	public async void DeferredGotoScene2 (string path) {
		animationPlayer.Play("fade_in");
		await ToSignal(animationPlayer, "animation_finished");

		GetTree().ChangeSceneToFile(path);

		animationPlayer.Play("fade_out");
	}
}

