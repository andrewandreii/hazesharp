using Godot;
using System;

public partial class UICurrency : HBoxContainer
{
	public int coins;

	public Label label;

	public override void _Ready()
	{
		Haze.World.Ready += () =>
		{
			Haze.World.blob.coinsUpdated += updateCoins;
		};

		label = GetNode<Label>("Coins");
		updateCoins(0);
	}

	public void updateCoins(int coins)
	{
		label.Text = coins.ToString();
	}
}
