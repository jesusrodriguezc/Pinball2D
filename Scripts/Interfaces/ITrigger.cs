using Godot;
using System.Collections.Generic;

public interface ITrigger {
	public enum TriggerBehaviour {
		INSTANTANEOUS,
		STAY_AND_WAIT,
		STAY_AND_PRESS_KEY,
		STAY_AND_HOLD_KEY,
		STAY_PRESS_AND_WAIT,
		STAY_HOLD_AND_WAIT
	}

	public static readonly StringName HoldPercentage = "hold_perc";
	public static readonly StringName Instantaneous = "instantaneous";


	public void Trigger (Dictionary<StringName, object> args = null);
	public void ProcessInstantaneous ();
	public void ProcessWait ();
	public void ProcessPress (bool isPressed);
	public void ProcessHold (bool isPressed);
	public void ProcessPressWait (bool isPressed);
	public void ProcessHoldWait (bool isPressed);
}