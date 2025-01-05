using Godot;
using System;

public partial class SmallEnemy : Area2D, IEnemy
{
	public static PackedScene CoinScene;

	[Export]
	public EnemyType Type { get; set; }

	public int health;

	public IAI enemyAI;
	IAI.AIState previousState;

	public Sprite2D sprite;

	int frameTimer = 0;

	public enum AnimationSpriteSheet
	{
		Idle, Moving, Attacking, Hurt
	};

	void onPlayerTouch(Node2D body)
	{
		if (body is Blob blob)
		{
			if (blob.isDrilling && blob.GlobalPosition.Y < GlobalPosition.Y)
			{
				return;
			}

			blob.takeDamage(1);
		}
	}

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		CoinScene = GD.Load<PackedScene>("res://scenes/coin.tscn");

		foreach (Node2D child in GetChildren())
		{
			if (child is IAI ai)
			{
				enemyAI = ai;
				break;
			}
		}

		health = Type.health;
	}

	bool processAI = true;
	public override void _PhysicsProcess(double delta)
	{
		if (processAI)
		{
			Position = enemyAI.ai(delta);
			if (previousState != enemyAI.State)
			{
				frameTimer = 0;
				previousState = enemyAI.State;
			}

			AnimationSpriteSheet toPlayAnimation = AnimationSpriteSheet.Idle;
			if (enemyAI.State != IAI.AIState.Idle)
			{
				sprite.FlipH = enemyAI.getDirection().X > 0;
				toPlayAnimation = enemyAI.State == IAI.AIState.Attacking ? AnimationSpriteSheet.Attacking : AnimationSpriteSheet.Moving;
			}

			playAnimation(toPlayAnimation);
		}

		continueAnimation();
	}

	AnimationSpriteSheet currentAnimation = AnimationSpriteSheet.Idle;
	void playAnimation(AnimationSpriteSheet animation)
	{
		if (animation == currentAnimation)
		{
			return;
		}

		currentAnimation = animation;

		frameTimer = 0;
		sprite.Frame = Type.whichRow * sprite.Hframes + (int)animation * 2;
	}

	void continueAnimation()
	{
		++frameTimer;
		if (frameTimer > Type.animationFrameCountTo)
		{
			frameTimer = 0;
			sprite.Frame = Type.whichRow * sprite.Hframes - (sprite.Frame % 2 - 1) + (int)currentAnimation * 2;
		}
	}

	public void takeDamage(int amount)
	{
		if (Type.unkillable)
		{
			return;
		}

		health -= amount;
		sprite.Frame = 0;
		var timer = GetTree().CreateTimer(300);
		playAnimation(AnimationSpriteSheet.Hurt);
		processAI = false;
		GetTree().CreateTimer(0.2).Timeout += () => processAI = true;
		if (health < 0)
		{
			// TODO: remember which enemies died
			var coin = CoinScene.Instantiate<Coin>();
			coin.value = (uint)GD.RandRange(Type.coinDropRange.X, Type.coinDropRange.Y);
			coin.Position = Position;
			Haze.World.currentLevel.CallDeferred("add_child", coin);
			QueueFree();
		}
	}
}
