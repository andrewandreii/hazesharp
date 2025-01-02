using Godot;
using System;

public partial class EnemyProjectile : Area2D
{
	public Vector2 velocity = Vector2.Down * 50f;

	public void onPlayerHit(Node2D body)
	{
		if (body is Blob blob)
		{
			blob.takeDamage(1);
		}
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += new Vector2(velocity.X * (float)delta, velocity.Y * (float)delta);
	}
}
