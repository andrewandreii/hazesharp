using Godot;
using System;

public partial class World : Node2D
{
	[Export(PropertyHint.File, "*.tscn,")]
	public String levelPath;

	[Export]
	public int startDoor = 2;

	public Level currentLevel;
	public Blob blob;
	public BlobCam blobCam;

	public enum WorldState
	{
		Normal,
		LevelTransition
	};

	public WorldState state = WorldState.Normal;

	public void changeRoom(String levelName, int doorId)
	{
		GD.Print($"Supposed to change scene to {levelName} at {doorId}");

		GD.Print($"room transition while {blob.Position}");

		if (state == WorldState.LevelTransition)
		{
			return;
		}

		uint layer = blob.CollisionLayer;
		blob.CollisionLayer = 0;

		state = WorldState.LevelTransition;

		Level new_level = createLevel(levelName, doorId);
		currentLevel.QueueFree();

		currentLevel = new_level;

		CallDeferred("add_child", currentLevel);

		var timer = GetTree().CreateTimer(0.1);
		timer.Timeout += () =>
		{
			blob.CollisionLayer = layer;
			state = WorldState.Normal;
		};

		return;
	}

	public override void _Ready()
	{
		blob = GetNode<Blob>("Blob");
		blobCam = GetNode<BlobCam>("BlobCam");

		currentLevel = createLevel(levelPath, startDoor);
		AddChild(currentLevel);
	}

	Level createLevel(String levelPath, int doorId)
	{
		var level = GD.Load<PackedScene>(levelPath).Instantiate<Level>();
		level.init(changeRoom);

		level.Ready += () =>
		{
			Vector2 start, direction;
			(start, direction) = currentLevel.getRoomEnterWalk(doorId, 16);
			GD.Print($"player from {start} to {direction}");
			blob.Position = start + 17 * direction;
			setCameraLimits();
		};

		return level;
	}

	void setCameraLimits()
	{
		Vector2 margin = new Vector2(1, 1);

		blobCam.MinLimit = (currentLevel.levelRect.Position + margin) * 16;
		blobCam.MaxLimit = (currentLevel.levelRect.Position + currentLevel.levelRect.Size - margin) * 16;
	}

	public TileMapLayer getBasicTilemap()
	{
		return currentLevel.GetNode<TileMapLayer>("Basic");
	}

	public Blob getBlob()
	{
		return blob;
	}
}
