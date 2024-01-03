using Godot;
using System.Collections.Generic;

public class TriggerBehaviour {
	public enum TriggerBehaviourId {
		INSTANTANEOUS	= 0b0000,
		STAY_AND_WAIT	= 0b0001,

		STAY_AND_PRESS_KEY	= 0b0010,
		STAY_PRESS_AND_WAIT	= 0b0011,

		STAY_AND_HOLD_KEY	= 0b0110,
		STAY_HOLD_AND_WAIT	= 0b0111,

		STILL_STAY_AND_WAIT		 = 0b1001,
		STILL_STAY_AND_PRESS_KEY = 0b1010,
		STILL_STAY_PRESS_AND_WAIT= 0b1011,
		STILL_STAY_AND_HOLD_KEY	 = 0b1110,
		STILL_STAY_HOLD_AND_WAIT = 0b1111
	}

	public TriggerBehaviourId Id { 
		get 
		{
			int triggerBehaviourInt = 0;
			triggerBehaviourInt |= (WaitInArea ? 0b1 : 0);
			triggerBehaviourInt |= (PressButton ? 0b10 : 0);
			triggerBehaviourInt |= (HoldButton ? 0b100 : 0);
			triggerBehaviourInt |= (Stopped ? 0b1000 : 0);

			return (TriggerBehaviourId)triggerBehaviourInt;
		}
	}
	public bool WaitInArea { get; set; }
	public bool PressButton { get; set; }
	private bool _holdButton;
	public bool HoldButton { get { return _holdButton; } set { _holdButton = value; PressButton = value; } }
	public bool Stopped { get; set; }

	public TriggerBehaviour(TriggerBehaviourId triggerId) {
		uint id = (uint)triggerId;

		WaitInArea = (id & 1) == 1;
		PressButton = (id & (1 << 1)) == (1 << 1);
		HoldButton = (id & (1 << 2)) == (1 << 2);
		Stopped = (id & (1 << 3)) != (1 << 3);
	}

}
public interface ITrigger {
	
	public static readonly StringName HOLD_PERCENTAGE = "hold_perc";
	public static readonly StringName INSTANTANEOUS = "instantaneous";
	public static readonly StringName ENTERING = "entering";

	// Teleport
	public static readonly StringName ACTIVATOR = "activator";
	public static readonly StringName POSITION = "position";
	public static readonly StringName VELOCITY = "velocity";


	public void Trigger (Dictionary<StringName, object> args = null);
	//public void ProcessInstantaneous ();
	//public void ProcessWait ();
	//public void ProcessPress (bool isPressed);
	//public void ProcessHold (bool isPressed);
	//public void ProcessPressWait (bool isPressed);
	//public void ProcessHoldWait (bool isPressed);
	public void OnTargetStopped (Node2D node);
}