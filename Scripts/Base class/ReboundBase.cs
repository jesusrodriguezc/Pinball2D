using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public abstract partial class ReboundBase : StaticBody2D, IActionable{
	protected Area2D _collisionArea;
	protected AnimationPlayer _animationPlayer;

	#region Components
	public SpriteManagerComponent spriteManagerComponent;
	public ScoreComponent scoreComponent;
	protected AudioComponent _audioComponent;
	protected GpuParticles2D particleSystem;
	#endregion

	[Export] public AudioStream HitStream;
	public readonly StringName HIT = "Hit";


	[Signal] public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	[Export] public float HitPower { get; set; }
	[Export] public float Score { get; set; }
	public bool IsCollisionEnabled { get; set; } = true;
	protected Godot.RandomNumberGenerator randomNumberGenerator;

	public override void _Ready () {
		randomNumberGenerator = new();
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		particleSystem = GetNodeOrNull<GpuParticles2D>("ParticleSystem");
		_collisionArea = GetNode<Area2D>("CollisionArea");

		_collisionArea.BodyEntered += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { ITrigger.ENTERING, true }, { ITrigger.ACTIVATOR, node } } });
		_collisionArea.BodyExited += (node) => Action(new EventData() { Sender = node, Parameters = new Dictionary<StringName, object>() { { ITrigger.ENTERING, false }, { ITrigger.ACTIVATOR, node } } });
		_collisionArea.CollisionLayer = CollisionLayer;
		_collisionArea.CollisionMask = CollisionMask;

		spriteManagerComponent = GetNodeOrNull<SpriteManagerComponent>("SpriteManagerComponent");
		spriteManagerComponent?.ChangeTexture(0);

		scoreComponent = GetNodeOrNull<ScoreComponent>("ScoreComponent");

		if (scoreComponent != null) {
			scoreComponent.BaseScore = Score;
		}
		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (_audioComponent != null) {
			_audioComponent.AddAudio(HIT, HitStream);
		}
	}
	public virtual void Collision (Node2D node) {

		Vector2 dirImpulso = CalculateImpulseDirection(node).Rotated(randomNumberGenerator.RandfRange(-0.1f, 0.1f));
		if (_animationPlayer?.HasAnimation("on_collision") ?? false) {
			_animationPlayer.Play("on_collision");
		}

		_audioComponent?.Play(HIT, AudioComponent.SFX_BUS);
		
		EmitSignal(SignalName.Impulse, node, dirImpulso * HitPower);
		scoreComponent?.AddScore();

	}

	public virtual void Release (Node2D node) {
		// Replace with function body.

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		Ball currBall = (Ball)node;
		if (PinballController.Instance.Ball != currBall) {
			GD.Print("La pelota no esta en la lista de pelotas.");
			return;
		}
		EmitParticles(node);

		if (_animationPlayer?.HasAnimation("on_release") ?? false) {
			_animationPlayer.Play("on_release");
		}


	}

	public abstract Vector2 CalculateImpulseDirection (Node2D node);
	public abstract void EmitParticles (Node2D node);

	public void Action (EventData data) {
		if (!IsCollisionEnabled) {
			return;
		}

		if (data.Sender is not IActor) {
			return;
		}
		bool isEntering = false;
		if (data.Parameters.TryGetValue(ITrigger.ENTERING, out var entering)) {
			isEntering = (bool)entering;
		}

		if (isEntering) {
			Collision(data.Sender);
		} else {
			Release(data.Sender);
		}
	}


	public void EnableCollision (bool enable) {
		IsCollisionEnabled = enable;
	}
}

