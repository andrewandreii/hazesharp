using Godot;
using System;

public partial class World : Node2D
{
	[Export(PropertyHint.File, "*.tscn,")]
	public String levelPath;

	public Level current_level;
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

		state = WorldState.LevelTransition;

		Level new_level = createLevel(levelName);
		new_level.init(roomChanged);
		current_level.QueueFree();

		current_level = new_level;
		current_level.Show();
		blob.Show();

		CallDeferred("add_child", current_level);

		var timer = GetTree().CreateTimer(0.1);
		timer.Timeout += () => state = WorldState.Normal;

		current_level.Ready += () =>
		{
			Vector2 start, end;
			(start, end) = current_level.getRoomEnterWalk(doorId, 16);
			GD.Print($"player from {start} to {end}");
			blob.Position = end;
			setCameraLimits();
		};

		return;
	}

	public override void _Ready()
	{
		current_level = createLevel(levelPath);
		current_level.init(roomChanged);
		AddChild(current_level);
		current_level.Show();
		current_level.Ready += () => { setCameraLimits(); };

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
		blobCam.MinLimit = current_level.levelRect.Position * 16;
		blobCam.MaxLimit = (current_level.levelRect.Position + current_level.levelRect.Size) * 16;

		GD.Print(blobCam.MinLimit, blobCam.MaxLimit);
	}
}
