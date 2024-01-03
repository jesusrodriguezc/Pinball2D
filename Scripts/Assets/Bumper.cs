using Godot;
using System.Collections.Generic;

public partial class Bumper : StaticBody2D, IActionable  {
	private Area2D _collisionArea;
	private AnimationPlayer _animationPlayer;

	#region Components
	private SpriteManagerComponent _spriteManager;
	private ScoreComponent _scoreComponent;
	private AudioComponent _audioComponent;
	#endregion

	public readonly StringName HIT = "Hit";


	[Signal] public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export] public float HitPower { get; set; }
	[Export] public float Score { get; set; }
	public bool IsCollisionEnabled { get; set; } = true;

	public override void _Ready () {
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

		_collisionArea = GetNode<Area2D>("CollisionArea");

		_collisionArea.BodyEntered += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { ITrigger.ENTERING, true }, { ITrigger.ACTIVATOR, node } } });
		_collisionArea.BodyExited += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { ITrigger.ENTERING, false }, { ITrigger.ACTIVATOR, node } } });
		_collisionArea.CollisionLayer = CollisionLayer;

		_spriteManager = GetNodeOrNull<SpriteManagerComponent>("SpriteManagerComponent");
		_spriteManager.ChangeTexture(0);

		_scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");

		if (_scoreComponent != null) {
			_scoreComponent.BaseScore = Score;
		}
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(HIT, ResourceLoader.Load<AudioStream>("res://SFX/bumper_hit.wav"));
		}

	}

	public void Action (EventData data) {

		GD.Print($"Funciona? {IsCollisionEnabled}");
		if (!IsCollisionEnabled) {
			return;
		}
		bool isEntering = false;
		if (data.Parameters.TryGetValue(ITrigger.ENTERING, out var entering)) {
			isEntering = (bool) entering;
		}

		if (isEntering) {
			Collision(data.Sender);
		}
		else {
			Release(data.Sender);
		}
	}
	private void Collision (Node2D node) {

		if (node is not IActor) {
			return;
		}

		Vector2 dirImpulso = (node.GlobalPosition - GlobalPosition).Normalized();

		_animationPlayer?.Play("on_collision");
		_audioComponent?.Play(HIT, AudioComponent.SFX_BUS);
		_scoreComponent?.AddScore();
		EmitSignal("Impulse", node, dirImpulso * HitPower);
	}

	private void Release (Node2D node) {

		if (node is not IActor actor) {
			return;
		}

		_animationPlayer?.Play("on_release");
	}

	public void EnableCollision(bool enable) {
		IsCollisionEnabled = enable;
	}
}

