using Godot;
using System;

public partial class Enemy : Area2D
{
	public enum EnemySize
	{
		Small, Big
	};

	[Export]
	public EnemySize enemySize;

	IAI enemyAI;
	IAI.AIState previousState;

	public Sprite2D sprite;

	[Export]
	public int enemyType = 0;

	[Export]
	public int animationFrameCountTo = 7;
	int frameTimer = 0;

	public enum AnimationSpriteSheet
	{
		Idle, Moving, Attacking
	};

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");

		foreach (Node2D child in GetChildren())
		{
			if (child is IAI ai)
			{
				enemyAI = ai;
				break;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
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

		++frameTimer;
		if (frameTimer > animationFrameCountTo)
		{
			frameTimer = 0;
			playAnimation(toPlayAnimation);
		}
	}

	void playAnimation(AnimationSpriteSheet animation)
	{
		sprite.Frame = enemyType * sprite.Hframes - (sprite.Frame % 2 - 1) + (int)animation * 2;
	}
}
