using Godot;
using System;

public partial class PowerUp : Area2D
{
	public enum PowerUpType
	{
		HealthUp, AttackUp, DoubleJump, Coins
	}

	[Export]
	public PowerUpType type;

	[Export]
	public uint coinValue;

	public Sprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");

		if (WorldProgression.isMarkedAsCollected(this))
		{
			QueueFree();
		}

		sprite.Frame = (int)type;
	}

	public void onPowerUpCollected(Node2D body)
	{
		WorldProgression.markAsCollected(this);

		var blob = Haze.World.getBlob();

		switch (type)
		{
			case PowerUpType.HealthUp:
				blob.maxHealth += 1;
				blob.health = blob.maxHealth;
				blob.takeDamage(0);
				break;
			case PowerUpType.AttackUp:
				blob.damage += 3;
				break;
			case PowerUpType.DoubleJump:
				blob.hasDoubleJump = true;
				break;
			case PowerUpType.Coins:
				blob.addCoins(coinValue);
				break;
		}

		QueueFree();
	}
}
