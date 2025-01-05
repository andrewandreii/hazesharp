using Godot;
using System;

public partial class Menu : Control
{
	public String worldPath = "res://scenes/world.tscn";

	public void onNewGamePressed()
	{
		GetTree().ChangeSceneToFile(worldPath);
	}

	public void onContinuePressed()
	{
		Haze.loadSave(0);
		GetTree().ChangeSceneToFile(worldPath);
	}

	public void onExitPressed()
	{
		GetTree().Quit();
	}
}
