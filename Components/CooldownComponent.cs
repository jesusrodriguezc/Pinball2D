using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class CooldownComponent : Node {
	public Timer Timer { get; private set; }
	public double TimeLeft => Timer.TimeLeft;
	public bool IsOnCooldown => Timer.TimeLeft > 0;
	public bool Disabled;

	public override void _Ready () {
		Timer = GetNode<Timer>("Timer");
	}

	public void SetCooldown(double cooldown = -1f) {
		Disabled = cooldown <= 0;
		if (Disabled) {
			return;
		}
			
		Timer.WaitTime = cooldown;
	}

	public void ApplyCooldown (double cooldown = -1f) {
		if (Timer == null) {
			return;
		}

		if (Timer.TimeLeft > 0) {
			return;
		}

		Disabled = cooldown <= 0;
		if (Disabled) {
			return;
		}

		Timer.Start(cooldown);
	}

}

