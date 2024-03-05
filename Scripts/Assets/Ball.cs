using Godot;
using Pinball.Scripts.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Ball : RigidBody2D, IActor{
	public enum Status {
		GAMEOVER = -2,
		DEAD = -1,
		IDLE = 0,
		MOVING = 1,
		PAUSED = 2
	}

	[Signal]
	public delegate void DeathEventHandler (Ball ball);

	private PinballController pinballController;
	private EventManager eventManager;
	[Export]
	public int DebugPower { get; set; } = 3000;
	private bool DebugTeleport = false;

	[Export]
	public float MaxVelocity { get; set; }
	public List<TriggerZone> CurrentTriggerZones { get; set; } = new List<TriggerZone>();

	public Status currentStatus;
	public Status previousStatus;
	[Export] private LayerId currentLayer;
	private AnimationPlayer animationPlayer;
	private Vector2 initialPosition;
	private AudioComponent _audioComponent;
	private readonly StringName DEAD = "Dead";

	internal Vector2 storedVelocity;
	private Vector2 storedPosition;
	private uint storedCollisionLayer;
	private uint storedCollisionMask;
	private float storedGravityScale;

	public override void _Ready () {

		pinballController = GetNode<PinballController>("/root/Pinball");
		eventManager = GetNodeOrNull<EventManager>("/root/EventManager");

		InitSignalConnections();

		_audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		_audioComponent?.AddAudio(DEAD, ResourceLoader.Load<AudioStream>("res://SFX/deathzone_enter.wav"));

		animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		animationPlayer.AnimationFinished += (animation) => GD.Print($"Ball -> Animacion {animation} terminada");
		initialPosition = GlobalPosition;
		currentStatus = Status.IDLE;
		previousStatus = Status.IDLE;

		SetLevel(currentLayer);
	}

	private void InitSignalConnections () {
		var rebounds = Nodes.findByClass<ReboundBase>(GetTree().Root);

		foreach (var rebound in rebounds) {
			rebound.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
		}

		var flippers = Nodes.findByClass<Flipper>(GetTree().Root);
		foreach (var flipper in flippers) {
			flipper.Impulse += (nodeAffected, impulse) => { if (nodeAffected == this) ApplyImpulse(impulse); };
		}

		//var wormholes = Nodes.findByClass<Wormhole>(GetTree().Root);
		//foreach (var wormhole in wormholes) {
		//	wormhole.EnteringBall += EnteringWormhole;
		//	wormhole.ExitingBall += ExitingWormhole;
		//}
	}

	public override void _Input (InputEvent @event) {

		if (currentStatus.IsNotIn(Status.IDLE, Status.MOVING)) {
			return;
		}

		if (@event is InputEventMouseButton mouseButton) {
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.IsPressed()) {
				Vector2 mousePosition = GetGlobalMousePosition();
				Vector2 forceDirection = (mousePosition - GlobalPosition).Normalized();
				ForceMove(forceDirection, DebugPower);
			}
		}

		if (@event is InputEventKey key) {
			if (key.Keycode.Equals(Key.Enter) && key.IsPressed()) {
				pinballController.TiltShake();
			}		
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess (double delta) {

		if (currentStatus == Status.DEAD && pinballController.LivesLeft > 0) {
			currentStatus = Status.IDLE;
		}

		if (!GetCollisionMaskValue(5) && LinearVelocity.LengthSquared() > (MaxVelocity * MaxVelocity)) {
			LinearVelocity = LinearVelocity.LimitLength(MaxVelocity);
		}

		if (OutOfLimits()) {
			GD.PushWarning($"Out of limits. Trying to teleport to {initialPosition}");
			Reset();

		}
	}

	private bool OutOfLimits () {
		return !GlobalPosition.IsFinite();
	}

	private void ApplyImpulse (Vector2 impulse) {
		LinearVelocity = new Vector2(0f, 0f);
		ApplyCentralImpulse(impulse);
	}

	public void ForceMove (Vector2 direction, float impulsePower) {
		ApplyCentralImpulse(direction * impulsePower);

	}

	public void Die () {
		_audioComponent?.Play(DEAD, AudioComponent.SFX_BUS);

		currentStatus = Status.DEAD;

		EmitSignal(SignalName.Death, this);
	}

	private void Reset () {
		GD.Print("Reseteando pelota", initialPosition, Vector2.Zero);
		Stop();
		Teleport(initialPosition, Vector2.Zero);
		Resume();
	}

	public void Teleport(Vector2 position, Vector2 velocity) {
		LinearVelocity = velocity;
		GlobalPosition = position;
	}

	public void SetLevel (LayerId layerId) {

		int level = LayerManager.LayerId2Int(layerId);
		currentLayer = layerId;
		if (level == -1) { return; }

		CollisionMask &= 0b0011;

		SetCollisionMaskValue(level, true);

		ZIndex = LayerManager.GetZLevel(level);
		CollisionLayer = LayerManager.GetBallLayer(level);

		eventManager.SendMessage(this, LayerManager.GetActionablesFromLayer(layerId, GetTree().Root), EventManager.EventType.DISABLE, null);

	}

	#region IActor definitions
	public void Pause () {
		storedVelocity = LinearVelocity;
		storedPosition = GlobalPosition;

		previousStatus = currentStatus;
		currentStatus = Status.PAUSED;

	}

	public void Unpause () {
		
		GlobalPosition = storedPosition;
		LinearVelocity = storedVelocity;

		currentStatus = previousStatus;
		previousStatus = Status.PAUSED;

	}

	public void Stop () {
		LinearVelocity = Vector2.Zero;
		storedGravityScale = GravityScale;
		GravityScale = 0.0f;
	}

	public void Resume () {
		GravityScale = storedGravityScale;
	}


	public void IgnoreCollision (bool ignoreCollision) {

		if (ignoreCollision) {
			storedCollisionLayer = CollisionLayer;
			CollisionLayer = 0;
			storedCollisionMask = CollisionMask;
			CollisionMask = 0;
		} else 
		{
			CollisionLayer = storedCollisionLayer;
			CollisionMask = storedCollisionMask;
		}
	}

	public void EnteringWormhole () {
		GD.Print("Inside EnteringWormhole");

		animationPlayer.Play("entering_wormhole");
		Hide();
		Stop();
		IgnoreCollision(true);
		Teleport(new Vector2(100, 100), Vector2.Zero);
	}

	public void ExitingWormhole (Vector2 finalPosition) {
		GD.Print("Inside ExitingWormhole");

		Show();
		Resume();
		Teleport(finalPosition, Vector2.Zero);
		IgnoreCollision(false);
		//animationPlayer.Play("exiting_wormhole");

	}
	#endregion
}
