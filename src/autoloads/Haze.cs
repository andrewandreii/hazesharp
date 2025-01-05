using Godot;
using System;

public partial class Haze : Node2D
{
	public static Haze Instance;
	public override void _Ready()
	{
		Instance = this;
	}

	static World _world = null;
	public static World World
	{
		get
		{
			if (_world is not null)
			{
				return _world;
			}
			_world = Instance.GetNode<World>("/root/World");
			return _world;
		}
	}

	static GameInfo _info = null;
	public static GameInfo Info
	{
		get
		{
			if (_info is not null)
			{
				return _info;
			}
			_info = Instance.GetNode<GameInfo>("/root/World/UI/GameInfo");
			return _info;
		}
	}

	public static bool SaveLoaded { get; set; }
	public static SaveSystem.BlobData blobData;
	public static SaveSystem.WorldData worldData;
	public static void loadSave(int slot)
	{
		(blobData, worldData) = SaveSystem.loadFromSlot(slot);
		SaveLoaded = true;
	}
}
