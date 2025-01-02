using Godot;
using System;

public partial class BugPatrolAI : Node2D, IAI
{
	RayCast2D ray;

	public Vector2 dir = Vector2.Left;

	[Export]
	public float speed = 40f;

	[Export]
	public float waitTime = 1;

	public IAI.AIState state;
	public IAI.AIState State { get => state; set => state = value; }

	public override void _Ready()
	{
		Enemy parent = GetParent<Enemy>();
		ray = GetNode<RayCast2D>("RayCast2D");

		state = IAI.AIState.MovingX;

		switch (parent.type.size)
		{
			case EnemyType.EnemySize.Small:
				ray.Position = new Vector2(-11, -6);
				break;
			case EnemyType.EnemySize.Big:
				ray.Position = new Vector2(-11, -6);
				break;
		}
	}

	public Vector2 ai(double delta)
	{
		if (state != IAI.AIState.MovingX)
		{
			return GlobalPosition;
		}

		if (!ray.IsColliding())
		{
			dir = -dir;
			ray.Position = new Vector2(-ray.Position.X, ray.Position.Y);
			state = IAI.AIState.Idle;
			GetTree().CreateTimer(waitTime).Timeout += () => state = IAI.AIState.MovingX;
			return GlobalPosition;
		}

		return GlobalPosition + dir * speed * (float)delta;
	}

	public Vector2 getDirection()
	{
		return dir;
	}
}
