using Godot;
using System;

public partial class TutorialHint : Area2D
{
	[Export]
	public String hintText;

	public void onPlayerEntered(Node2D body)
	{
		Haze.Info.showInfo(hintText);
	}
}
