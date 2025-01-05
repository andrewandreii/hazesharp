using Godot;
using System;

public partial class BackHomeDoor : Area2D, IInteractable
{
	public Sprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public void select()
	{
		sprite.Frame = 1;
	}

	public void deselect()
	{
		sprite.Frame = 0;
	}

	public void use()
	{
		Haze.World.changeRoom(Haze.worldData.levelPath, 0);
	}
}
