using Godot;
using System;

public partial class World : Node2D
{
	[Export(PropertyHint.File, "*.tscn,")]
	public String levelPath;

	public Level currentLevel;
	public Blob blob;
	public BlobCam blobCam;

	public enum WorldState
	{
		Normal,
		LevelTransition
	};

	public WorldState state = WorldState.Normal;

	public void roomChanged(String levelName, int doorId)
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

		Level new_level = createLevel(levelName);
		new_level.init(roomChanged);
		currentLevel.QueueFree();

		currentLevel = new_level;
		currentLevel.Show();
		blob.Show();

		CallDeferred("add_child", currentLevel);

		var timer = GetTree().CreateTimer(0.1);
		timer.Timeout += () =>
		{
			blob.CollisionLayer = layer;
			state = WorldState.Normal;
		};

		currentLevel.Ready += () =>
		{
			Vector2 start, direction;
			(start, direction) = currentLevel.getRoomEnterWalk(doorId, 16);
			GD.Print($"player from {start} to {direction}");
			blob.Position = start + 17 * direction;
			setCameraLimits();
		};

		return;
	}

	public override void _Ready()
	{
		currentLevel = createLevel(levelPath);
		currentLevel.init(roomChanged);
		AddChild(currentLevel);
		currentLevel.Show();
		currentLevel.Ready += () => { setCameraLimits(); };

		blob = GetNode<Blob>("Blob");
		blobCam = GetNode<BlobCam>("BlobCam");
	}

	Level createLevel(String levelPath)
	{
		var levelScene = GD.Load<PackedScene>(levelPath);
		return levelScene.Instantiate<Level>();
	}

	void setCameraLimits()
	{
		blobCam.MinLimit = currentLevel.levelRect.Position * 16;
		blobCam.MaxLimit = (currentLevel.levelRect.Position + currentLevel.levelRect.Size) * 16;

		GD.Print(blobCam.MinLimit, blobCam.MaxLimit);
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
