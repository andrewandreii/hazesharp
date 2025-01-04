using Godot;
using System;

public partial class Coin : Area2D
{
	public int value = 10;

	public Sprite2D sprite;

	public bool hoverEffectIsUp = true;
	public int hoverEffectTicks = 0;
	public int hoverEffectMaxTicks = 20;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Frame = 0;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 pos = Position;
		pos.Y += (hoverEffectIsUp ? -10 : 10) * (float)delta;
		++hoverEffectTicks;
		if (hoverEffectTicks > hoverEffectMaxTicks)
		{
			hoverEffectTicks = 0;
			hoverEffectIsUp = !hoverEffectIsUp;
		}
		Position = pos;
	}

	public void onBodyEntered(Node2D body)
	{
		if (body is Blob blob)
		{
			blob.addCoins(value);
			QueueFree();
		}
	}
}
