using Godot;
using System;

public partial class SpriteManagerComponent : Node
{
	private Sprite2D Sprite;

	[Export] CompressedTexture2D[] animatedTextures;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Sprite = GetNode<Sprite2D>("Sprite2D");
		Owner = GetParent<Node2D>();

		if (animatedTextures.Length == 0) {
			GD.PrintErr($"There is no sprite assigned to {Sprite.Name}");
			return;
		}
	}

	public void ChangeTexture(int index) {
		if (animatedTextures != null && animatedTextures.Length > index) {
			Sprite.Texture = animatedTextures[index];
		}
	}
}
