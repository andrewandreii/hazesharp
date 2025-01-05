using Godot;
using System;

public partial class FlyingEnemyAi : Node2D, IAI
{
	IAI.AIState state;
	public IAI.AIState State { get => state; set => state = value; }
	bool fleeing = false;
	bool chasing = false;

	TileMapLayer tilemap;
	Blob blob;

	int tilemapSize;

	[Export]
	public float speed = 80f;

	Vector2 dir;
	Vector2 lastValidTargetPosition;

	public int hoverEffectTimer = 0;
	public int hoverEffectFrames = 20;
	public bool hoverEffectIsUp = true;

	public PackedScene proj_scene;

	public Timer attackCooldown;
	public bool canAttack = true;

	public Vector2 targetBias;

	public override void _Ready()
	{
		tilemap = Haze.World.getBasicTilemap();
		blob = Haze.World.getBlob();

		tilemapSize = tilemap.TileSet.TileSize.X;

		proj_scene = GD.Load<PackedScene>("res://scenes/enemy/enemy_projectile.tscn");

		attackCooldown = GetNode<Timer>("AttackCooldown");
		attackCooldown.Timeout += () => canAttack = true;

		targetBias = new Vector2(GD.RandRange(-3, 3), GD.RandRange(-3, 3));
	}

	public void onObjectSighted(Node2D body)
	{
		if (body is Blob)
		{
			chasing = true;
		}
	}

	public void chasePlayer()
	{
		chasing = true;
	}

	public void spawnProjectile()
	{
		var proj = proj_scene.Instantiate<EnemyProjectile>();
		proj.Position = GlobalPosition;
		Haze.World.AddChild(proj);
	}

	public Vector2 ai(double delta)
	{
		Vector2 offset = new Vector2(0, hoverEffectIsUp ? -14f : 14f) * (float)delta;
		++hoverEffectTimer;
		if (hoverEffectTimer > hoverEffectFrames)
		{
			hoverEffectIsUp = !hoverEffectIsUp;
			hoverEffectTimer = 0;
		}

		if (!chasing)
		{
			return GlobalPosition + offset;
		}

		Vector2 targetPosition = new Vector2(blob.Position.X, blob.Position.Y - tilemapSize * 2) + targetBias;
		if (tilemap.GetCellTileData(tilemap.LocalToMap(targetPosition)) is not null)
		{
			targetPosition = lastValidTargetPosition;
			state = IAI.AIState.Idle;
		}
		else
		{
			state = IAI.AIState.Attacking;
			lastValidTargetPosition = targetPosition;
		}

		if (Mathf.Abs(GlobalPosition.X - targetPosition.X) < 2 && Mathf.Abs(GlobalPosition.Y - targetPosition.Y) < 30)
		{
			if (state == IAI.AIState.Attacking)
			{
				if (canAttack)
				{
					spawnProjectile();
					canAttack = false;
					attackCooldown.Start();
				}
			}

			return GlobalPosition + offset;
		}

		dir = GlobalPosition.DirectionTo(targetPosition);
		state = IAI.AIState.MovingXY;
		return GlobalPosition + dir * speed * (float)delta + offset;
	}

	public Vector2 getDirection()
	{
		return dir.X < 0 ? Vector2.Right : Vector2.Left;
	}
}
