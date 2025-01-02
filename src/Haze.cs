using Godot;
using System;

public partial class Haze : Node2D
{
	public static Haze Instance;
	public override void _Ready()
	{
		Instance = this;
	}

	static World _world = null;
	public static World World
	{
		get
		{
			if (_world is not null)
			{
				return _world;
			}
			_world = Instance.GetNode<World>("/root/World");
			return _world;
		}
	}
}
