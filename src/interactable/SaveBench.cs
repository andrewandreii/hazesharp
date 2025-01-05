using Godot;
using System;

public partial class SaveBench : Area2D, IInteractable
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
		Blob blob = Haze.World.getBlob();
		blob.health = blob.maxHealth;
		blob.takeDamage(0);
		SaveSystem.saveToSlot(0);
		Haze.Info.showInfo("Game saved.");
	}
}
