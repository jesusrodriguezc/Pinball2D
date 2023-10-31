using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SignalConnection {

	private StringName _receiver; 
	public StringName Receiver { get { return _receiver; } protected set { _receiver = value; } }
	private StringName _methodName;
	public StringName MethodName { get { return _methodName; } protected set { _methodName = value; } }

	public SignalConnection (StringName receiver, StringName methodName) { 
		_receiver = receiver;
		_methodName = methodName;
	}
}

