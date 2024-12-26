using Godot;
using System;

public partial class CheckPitfallComp : RayCast2D
{
	public Enemy parent;

	public override void _Ready()
	{
		parent = GetParent<Enemy>();
	}
}
