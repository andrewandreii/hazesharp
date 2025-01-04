using Godot;
using System;

[GlobalClass]
public partial class EnemyType : Resource
{
	public enum EnemySize
	{
		Small, Big
	};

	[Export]
	public int health;

	[Export]
	public bool unkillable = false;

	[Export]
	public EnemySize size;

	[Export]
	public int whichRow;

	[Export]
	public int animationFrameCountTo = 7;

	[Export]
	public Vector2I coinDropRange;

	public EnemyType() { }
}
