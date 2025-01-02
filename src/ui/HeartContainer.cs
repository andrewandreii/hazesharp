using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class HeartContainer : HBoxContainer
{
	Blob blob;
	List<TextureRect> hearts;

	public override void _Ready()
	{
		Haze.World.Ready += () =>
		{
			blob = Haze.World.getBlob();
			blob.tookDamage += updateHealth;
		};

		hearts = GetChildren().Cast<TextureRect>().ToList();
	}

	public void updateHealth()
	{
		GD.Print("current health: ", blob.health);
		int health = blob.health;
		foreach (TextureRect heart in hearts)
		{
			if (health > 0)
			{
				heart.Show();
			}
			else
			{
				heart.Hide();
			}
			--health;
		}
	}
}
