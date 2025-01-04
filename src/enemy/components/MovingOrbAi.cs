using Godot;
using System;

public partial class MovingOrbAi : Node2D, IAI
{
	public IAI.AIState state = IAI.AIState.MovingY;
	public IAI.AIState State { get => state; set => state = value; }

	public Vector2 dir = Vector2.Up;

	public TileMapLayer level;

	public Vector2 target;

	[Export]
	public float speed;
	[Export]
	public float waitTime = 0.7f;

	public Vector2I cellOffset;

	public override void _Ready()
	{
		level = Haze.World.getBasicTilemap();
		cellOffset = new Vector2I(0, level.TileSet.TileSize.Y / 2);

		target = level.MapToLocal(findTargetBlock()) + cellOffset;
	}

	public Vector2 ai(double delta)
	{
		if (state != IAI.AIState.MovingY)
		{
			return GlobalPosition;
		}

		if (GlobalPosition.DistanceTo(target) < 2)
		{
			state = IAI.AIState.Idle;
			GetTree().CreateTimer(waitTime).Timeout += () =>
			{
				state = IAI.AIState.MovingY;
				dir = -dir;
				target = level.MapToLocal(findTargetBlock()) + cellOffset;
			};
			return GlobalPosition;
		}

		return GlobalPosition + GlobalPosition.DirectionTo(target) * speed * (float)delta;
	}

	public Vector2I findTargetBlock()
	{
		Vector2I pos = level.LocalToMap(GlobalPosition);

		int dirY = (int)dir.Y;
		pos.Y += dirY;
		for (int i = 0; i < 10; ++i, pos.Y += dirY)
		{
			if (level.GetCellTileData(pos) is not null)
			{
				pos.Y -= dirY;
				break;
			}
		}

		return pos;
	}

	public Vector2 getDirection()
	{
		return dir;
	}
}
