using Godot;
using System;

public partial class SaveSystem
{
	public struct BlobData
	{
		public int maxHealth;
		public uint coins;
		public uint damage;
		public bool hasDoubleJump;
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
		file.Store32(blob.damage);
		file.Store32((uint)(blob.hasDoubleJump ? 1 : 0));
		file.Store32((uint)level.SceneFilePath.Length);
		file.StoreBuffer(level.SceneFilePath.ToAsciiBuffer());
		WorldProgression.saveToFile(file);

		file.Flush();

		Haze.worldData.levelPath = level.SceneFilePath;
	}

	public static (BlobData, WorldData) loadFromSlot(int slotNumber)
	{
		var filename = getFilename(slotNumber);
		if (!FileAccess.FileExists(filename))
		{
			OS.Alert("Could not load from save file. Have you ever saved before?", "ALERT");
			Haze.World.GetTree().Quit();
		}

		var file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
		BlobData blob;
		blob.maxHealth = (int)file.Get32();
		blob.coins = file.Get32();
		blob.damage = file.Get32();
		blob.hasDoubleJump = file.Get32() == 1;

		WorldData world;
		int pathLength = (int)file.Get32();
		world.levelPath = file.GetBuffer(pathLength).GetStringFromAscii();

		WorldProgression.loadFromFile(file);

		return (blob, world);
	}
}
