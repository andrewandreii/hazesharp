using Godot;
using Godot.Collections;

public partial class WorldProgression : Node
{
	public static Dictionary collectedPowerUps = new Dictionary();

	public static void saveToFile(FileAccess file)
	{
		file.StoreVar(collectedPowerUps);
	}

	public static void loadFromFile(FileAccess file)
	{
		collectedPowerUps = file.GetVar().As<Dictionary>();
	}

	public static void markAsCollected(PowerUp powerUp)
	{
		collectedPowerUps.Add(powerUp.GetPath(), true);
	}

	public static bool isMarkedAsCollected(PowerUp powerUp)
	{
		return collectedPowerUps.ContainsKey(powerUp.GetPath());
	}
}
