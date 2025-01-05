using Godot;
using System;

public partial class World : Node2D
{
	[Export(PropertyHint.File, "*.tscn,")]
	public String levelPath;

	[Export]
	public int startDoor = 2;

	public Level currentLevel;
	public int levelEnteredFrom;
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
		if (state == WorldState.LevelTransition)
		{
			return;
		}

		levelEnteredFrom = doorId;

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

		if (Haze.SaveLoaded)
		{
			levelPath = Haze.worldData.levelPath;
			startDoor = 0;
			blob.maxHealth = Haze.blobData.maxHealth;
			blob.health = blob.maxHealth;
			blob.takeDamage(0);
			blob.coins = Haze.blobData.coins;
			blob.hasDoubleJump = Haze.blobData.hasDoubleJump;
			blob.damage = Haze.blobData.damage;
		}

		currentLevel = createLevel(levelPath, startDoor);
		levelEnteredFrom = startDoor;
		AddChild(currentLevel);

		Haze.worldData.levelPath = levelPath;
	}

	Level createLevel(String levelPath, int doorId)
	{
		var level = GD.Load<PackedScene>(levelPath).Instantiate<Level>();
		level.init(changeRoom);

		level.Ready += () =>
		{
			setCameraLimits();
			restartLevel();
		};

		return level;
	}

	public void restartLevel()
	{
		Vector2 start, direction;
		(start, direction) = currentLevel.getRoomEnterWalk(levelEnteredFrom, 16);
		blob.Position = start + 17 * direction;
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
