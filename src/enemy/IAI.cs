using Godot;
using System;

public interface IAI
{
	public enum AIState
	{
		Idle, MovingX, MovingY, MovingXY, Attacking
	};
	public AIState State { get; set; }

	public Vector2 ai(double delta);
	public Vector2 getDirection();
}
