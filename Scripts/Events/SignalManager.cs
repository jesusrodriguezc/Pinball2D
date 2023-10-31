using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SignalManager : Node
{
	public enum ConnectionStatus {
		CONNECTED = 0,
		OBJECT_NOT_CONNECTED = -1,
		EVENT_NOT_CONNECTED = -2
	}
	#region Singleton
	// Singleton
	private readonly static SignalManager _instance = new SignalManager();
	private SignalManager () { }
	public static SignalManager Instance {
		get { return _instance; }
	}
	#endregion Singleton

	private static System.Collections.Generic.Dictionary<StringName, List<SignalConnection>> signalConnections = new System.Collections.Generic.Dictionary<StringName, List<SignalConnection>>();
	public override void _Ready () {
	}

	public static void SaveConnection (Node obj, StringName signalName, StringName methodName) {
		switch(GetConnectionStatus(obj.Name, signalName)) {
			case ConnectionStatus.EVENT_NOT_CONNECTED:
				signalConnections.Add(signalName, new List<SignalConnection>() { new SignalConnection(obj.Name, methodName)});
				break;

			case ConnectionStatus.OBJECT_NOT_CONNECTED:
				signalConnections[signalName].Add(new SignalConnection(obj.Name, methodName));
				break;
		}
	}

	public static void RemoveConnection (Node obj, StringName signalName) {
		if (HasConnection(obj.Name, signalName)) {
			signalConnections[signalName].RemoveAll(conn => conn.Receiver == obj.Name);
		}
	}

	public static bool HasConnection(StringName objectName, StringName signal) {
		return GetConnectionStatus(objectName, signal) == ConnectionStatus.CONNECTED;
	}

	public static ConnectionStatus GetConnectionStatus(StringName objectName, StringName signal) {
		if (signalConnections.TryGetValue(signal, out var objectList)) {
			if ( objectList.Find(conn => conn.Receiver == objectName) != null) {
				return ConnectionStatus.CONNECTED;
			}
			return ConnectionStatus.OBJECT_NOT_CONNECTED;
		}
		
		return ConnectionStatus.EVENT_NOT_CONNECTED;
		
	}
}
