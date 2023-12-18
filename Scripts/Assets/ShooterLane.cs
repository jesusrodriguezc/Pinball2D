using Godot;
using Pinball.Utils;

public partial class ShooterLane : Area2D {
	private Ball currBall;
	private Area2D collisionArea;

	#region Audio
	private AudioComponent audioComponent;
	public readonly StringName HIT = "Hit";
	public readonly StringName ENOUGH_POWER = "ENOUGH_POWER";

	#endregion
	[Export]
	public int ShotterLanePower { get; set; }

	[Signal]
	public delegate void ImpulseEventHandler (Node2D nodeAffected, Vector2 impulse);

	private bool isBallInside;

	private bool buttonHolded;  // Marca si se esta presionando el Enter o no.
	private double timeCountButtonHold; // Marca cuanto tiempo lleva presionada la tecla.
	[Export]
	public float MaxPowerHoldTime { get; set; } // Tiempo minimo que debe estar presionada la tecla para dar el impulso maximo.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready () {

		audioComponent = GetNodeOrNull<AudioComponent>("AudioComponent");
		if (audioComponent != null) {
			audioComponent.AddAudio(HIT, ResourceLoader.Load<AudioStream>("res://SFX/shooterlane_hit.wav"));
			audioComponent.AddAudio(ENOUGH_POWER, ResourceLoader.Load<AudioStream>("res://SFX/shooterlane_full.wav"));

		}
		BodyEntered += _OnBallEntered;
		BodyExited += _OnBallExited;

		isBallInside = true;
		DisableButtonHoldTimer();

	}

	public override void _Input (InputEvent @event) {
		if (@event is InputEventKey key) {
			switch (key.Keycode) {
				case Key.Space:
					if (!isBallInside) {
						break;
					}

					if (key.Pressed && !buttonHolded) {
						SetButtonHoldTimer();
						break;
					}


					if (!key.Pressed) {

						audioComponent?.Play(HIT, AudioComponent.SFX_BUS);
						float percButtonTime = (float)Mathf.Min(1, timeCountButtonHold / (double)MaxPowerHoldTime);
						float hitPower = ShotterLanePower * Mathx.FuncSmooth(percButtonTime);

						EmitSignal(SignalName.Impulse, currBall, Vector2.Up * hitPower);
						if (percButtonTime >= 0.35f) {
							audioComponent?.Play(ENOUGH_POWER, 0.1f);
						}
						DisableButtonHoldTimer();
					}
					break;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process (double delta) {
		if (buttonHolded) {
			timeCountButtonHold += delta;
		}
	}

	private void _OnBallEntered (Node node) {

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		currBall = (Ball)node;
		if (PinballController.Instance.Ball != currBall) {
			GD.PrintErr("La pelota no esta en la lista de pelotas.");
			return;
		}

		isBallInside = true;
	}

	private void _OnBallExited (Node node) {

		if (node.GetType() != typeof(Ball)) {
			return;
		}

		currBall = (Ball)node;
		if (PinballController.Instance.Ball != currBall) {
			GD.PrintErr("La pelota no esta en la lista de pelotas.");
			return;
		}

		isBallInside = false;
	}

	private void SetButtonHoldTimer () {
		timeCountButtonHold = 0;
		buttonHolded = true;
	}

	private void DisableButtonHoldTimer () {
		timeCountButtonHold = 0;
		buttonHolded = false;
	}
}
