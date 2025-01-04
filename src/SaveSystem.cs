using Godot;
using System;

public partial class SaveSystem
{
	public struct BlobData
	{
		public int maxHealth;
		public uint coins;
		public Vector2 position;
	}

	public struct WorldData
	{
		public String levelPath;
	}

	public const String saveFileTemplate = "user://save{0}.save";

	public static String getFilename(int slot)
	{
		return String.Format(saveFileTemplate, slot);
	}

	public static void saveToSlot(int slotNumber)
	{
		var file = FileAccess.Open(getFilename(slotNumber), FileAccess.ModeFlags.Write);

		Blob blob = Haze.World.blob;
		Level level = Haze.World.currentLevel;

		file.Store32((uint)blob.maxHealth);
		file.Store32(blob.coins);
		file.StoreFloat(blob.Position.X);
		file.StoreFloat(blob.Position.Y);
		file.StoreString(level.SceneFilePath);

		file.Flush();
	}

	public static (BlobData, WorldData) loadFromSlot(int slotNumber)
	{
		var filename = getFilename(slotNumber);
		if (!FileAccess.FileExists(filename))
		{
			OS.Alert("Could not load from save file, something went really wrong", "ALERT");
			Haze.World.GetTree().Quit();
		}

		var file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);

		BlobData blob;
		blob.maxHealth = (int)file.Get32();
		blob.coins = file.Get32();
		blob.position = new Vector2(file.GetReal(), file.GetReal());

		WorldData world;
		world.levelPath = file.GetAsText();

		return (blob, world);
	}
}
