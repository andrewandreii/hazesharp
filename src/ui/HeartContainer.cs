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
			blob.healthUpdated += updateHealth;
			updateHealth(blob.health);
		};

		hearts = GetChildren().Cast<TextureRect>().ToList();
	}

	public void updateHealth(int health)
	{
		GD.Print("current health: ", health);
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
