using Godot;
using System;

public partial class BossEnemy1 : Area2D, IEnemy
{
	public enum BossState
	{
		// Uses BugPatrolAI
		Patrol,
		// Looks for the middle of the arena to jump and cling to the platform
		JumpToPlatform,
		// Once clung to the platform, it starts spawning patterns of projectiles
		SprayProjectiles,
		// Starts spawning flying enemies
		SpawnFlyingEnemy
	}

	public BossState state;

	[Export]
	public EnemyType Type { get; set; }

	public int health;

	public BugPatrolAI enemyAI;

	public Sprite2D sprite;

	public PackedScene bugEnemyScene;

	public override void _Ready()
	{
		bugEnemyScene = GD.Load<PackedScene>("res://scenes/enemy/types/flying_enemy.tscn");

		sprite = GetNode<Sprite2D>("Sprite2D");

		foreach (Node2D child in GetChildren())
		{
			if (child is BugPatrolAI ai)
			{
				enemyAI = ai;
				break;
			}
		}

		health = Type.health;

		state = BossState.Patrol;
	}

	public bool spawnedBug = false;
	public Vector2 lastWaitPosition;
	public override void _PhysicsProcess(double delta)
	{
		if (state == BossState.Patrol)
		{
			if (!spawnedBug && Position.DistanceTo(lastWaitPosition) > 16 * 5)
			{
				spawnedBug = true;
				if (GD.Randf() < 0.3)
				{
					spawnBug();
				}
			}

			Position = enemyAI.ai(delta);
			sprite.FlipH = enemyAI.getDirection().X > 0;
			if (enemyAI.State == IAI.AIState.MovingX)
			{
				sprite.Frame = 1;
			}
			else
			{
				lastWaitPosition = Position;
				sprite.Frame = 0;
				spawnedBug = false;
			}
		}
	}

	public void spawnBug()
	{
		var bug = bugEnemyScene.Instantiate<SmallEnemy>();
		bug.Position = Position + Vector2.Down * 0;
		bug.Ready += () =>
		{
			FlyingEnemyAi ai = bug.enemyAI as FlyingEnemyAi;
			ai.speed = 30;
			ai.chasePlayer();
		};
		Haze.World.currentLevel.AddChild(bug);
	}

	public void onPlayerHit(Node2D body)
	{
		if (body is Blob blob)
		{
			if (blob.isDrilling && blob.GlobalPosition.Y < GlobalPosition.Y)
			{
				return;
			}

			blob.takeDamage(1);
		}
	}

	public void takeDamage(int amount)
	{
		health -= amount;

		if (health < 0)
		{
			QueueFree();
		}
	}
}
