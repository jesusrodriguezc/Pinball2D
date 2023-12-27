using Godot;
using Pinball.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Door : AnimatableBody2D, IActionable {

	public enum DoorStatus {
		NONE,
		CLOSED,
		OPENING,
		OPENED,
		CLOSING
	}

	public enum DoorBehaviour {
		SINGLE_USE,
		TOGGLE_ON_ACTION,
		CYCLE_ON_ACTION
	}

	[ExportCategory("Behaviour")]
	[Export] private DoorBehaviour Behaviour;

	private Queue<DoorStatus> ActionQueue = new Queue<DoorStatus>();
	[Export] private DoorStatus InitialStatus;
	private DoorStatus CurrentStatus;

	private Timer waitTimer;
	[Export] public double WaitingTime;


	[ExportCategory("Final transform")]

	private Transform2D OpenedTransform;
	private Transform2D ClosedTransform;

	[Export] public Vector2 FinalPosition;
	[Export] public float FinalRotation;
	[Export] public Vector2 FinalScale;

	[ExportCategory("Audio")]
	private AudioComponent AudioComponent;
	private StringName OPEN = "Open";
	[Export] private AudioStream OpenAudio;
	private StringName CLOSE = "Close";
	[Export] private AudioStream CloseAudio;

	private float _t = 0.0f;

	public override void _Ready () {
		switch (InitialStatus) {
			case DoorStatus.OPENED:
				OpenedTransform = Transform;
				ClosedTransform = new Transform2D(FinalRotation, FinalScale, 0f, FinalPosition);
				break;
			case DoorStatus.CLOSED:
				ClosedTransform = Transform;
				OpenedTransform = new Transform2D(FinalRotation, FinalScale, 0f, FinalPosition);
				break;
			default:
				throw new ArgumentException($"Door's InitialStatus parameter should be OPENED or CLOSED. Current value is {InitialStatus}");
		}

		CurrentStatus = InitialStatus;

		AudioComponent?.AddAudio(OPEN, OpenAudio);
		AudioComponent?.AddAudio(CLOSE, OpenAudio);

		if (WaitingTime > 0) {
			waitTimer = new Timer() {
				OneShot = true,
				WaitTime = WaitingTime
			};
			AddChild(waitTimer);

			if (Behaviour != DoorBehaviour.SINGLE_USE) {
				waitTimer.Timeout += () => { 
					if (CurrentStatus == DoorStatus.CLOSED) {
						Open();
					}	
					else {
						Close();
					}
				};
			}
		} else {
			Behaviour = DoorBehaviour.SINGLE_USE;
		}

	}

	public override void _Process (double delta) {
		// If the timer is running, just wait.
		if ((waitTimer?.TimeLeft ?? 0) > 0) {
			return;
		}

		// If the current status is final (Opened/Closed) and there is no action in progress, we enqueue action from the action queue (if any).
		if (CurrentStatus.IsIn(DoorStatus.OPENED, DoorStatus.CLOSED) && ActionQueue.Count > 0) {
			GD.Print($"There is {ActionQueue.Count} elements in the ActionQueue.");
			ExecuteAction(ActionQueue.Dequeue());
		}
	}
	public override void _PhysicsProcess (double delta) {

		if (waitTimer != null && waitTimer.TimeLeft > 0) {
			return;
		}
		switch (CurrentStatus) {
			case DoorStatus.OPENING:
				if (_t <= 1f) {
					Transform = Transform.InterpolateWith(OpenedTransform, _t);
					_t += (float)delta;
				}
				else {
					CurrentStatus = DoorStatus.OPENED;
					_t = 0f;
				}

				break;
			case DoorStatus.CLOSING:
				if (_t <= 1f) {
					Transform = Transform.InterpolateWith(ClosedTransform, _t);
					_t += (float)delta;
				} else {
					CurrentStatus = DoorStatus.CLOSED;
					_t = 0f;
				}
				break;
			default:
				break;

		}
	}

	public void Action (EventData data) {

		var previousActionToReceived = ActionQueue.LastOrDefault();
		if (previousActionToReceived == DoorStatus.NONE) {
			previousActionToReceived = CurrentStatus;
		}

		GD.Print($"{Name}.Action(). ActionQueue.Count = {ActionQueue.Count}");
		if (ActionQueue.Count > 1 || (ActionQueue.Count <= 1 && waitTimer.TimeLeft > 0)) {
			return;
		}

		switch (Behaviour) {
			case DoorBehaviour.SINGLE_USE:
				if (InitialStatus != previousActionToReceived) {
					return;
				}

				ActionQueue.Enqueue(InitialStatus == DoorStatus.CLOSED ? DoorStatus.OPENING : DoorStatus.CLOSING);
				return;
			case DoorBehaviour.TOGGLE_ON_ACTION:
				switch (previousActionToReceived) {
					case DoorStatus.CLOSED:
					case DoorStatus.CLOSING:
						ActionQueue.Enqueue(DoorStatus.OPENING);
						break;
					case DoorStatus.OPENED:
					case DoorStatus.OPENING:
						ActionQueue.Enqueue(DoorStatus.CLOSING);
						break;
				}
				break;
			case DoorBehaviour.CYCLE_ON_ACTION:
				ActionQueue.Enqueue(InitialStatus == DoorStatus.CLOSED ? DoorStatus.OPENING : DoorStatus.CLOSING);
				ActionQueue.Enqueue(InitialStatus == DoorStatus.CLOSED ? DoorStatus.CLOSING : DoorStatus.OPENING);

				break;
		}
	}
	
	public void ExecuteAction(DoorStatus nextStatus) {

		GD.Print($"{Name} changing status: {CurrentStatus} to {nextStatus}");

		switch (Behaviour) {
			case DoorBehaviour.SINGLE_USE:
				break;
			case DoorBehaviour.TOGGLE_ON_ACTION:
				break;
			case DoorBehaviour.CYCLE_ON_ACTION:
				if (waitTimer != null && InitialStatus != CurrentStatus) {
					waitTimer.Start();
					return;
				}
				break;
		}

		if (nextStatus == DoorStatus.OPENING) {
			Open();
		} else {
			Close();
		}
	}

	private void Close () {
		GD.Print("Should start closing");
		CurrentStatus = DoorStatus.CLOSING;

		AudioComponent?.Play(CLOSE, AudioComponent.SFX_BUS);
	}

	private void Open () {
		GD.Print("Should start opening");
		CurrentStatus = DoorStatus.OPENING;

		AudioComponent?.Play(OPEN, AudioComponent.SFX_BUS);
	}
}

