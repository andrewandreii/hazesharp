using Godot;
using System;

public partial class Blob : CharacterBody2D
{
	public const float Speed = 50.0f;
	public const float JumpVelocity = -400.0f;
	public const float MaxNormalSpeed = 200.0f;
	public const float MaxSpeed = 400.0f;
	public const float MaxFallSpeed = 400.0f;

	public bool isDrilling = false;
	public float drillGravityBoost = 1.4f;
	public float allowedSpeed = MaxNormalSpeed;
	public float speedBuff = 1.0f;
	public enum WalkType
	{
		Normal, Boosted, Slow
	};
	public WalkType currentWalk = WalkType.Normal;
	public WalkType CurrentWalk
	{
		get => currentWalk;
		set
		{
			if (currentWalk == value)
			{
				return;
			}

			switch (value)
			{
				case WalkType.Normal:
					allowedSpeed = MaxNormalSpeed;
					speedBuff = 1.0f;
					if (IsOnFloor()) anim.Play("walk");
					break;
				case WalkType.Boosted:
					allowedSpeed = MaxSpeed;
					speedBuff = 1.1f;
					if (IsOnFloor()) anim.Play("fast_walk");
					break;
				case WalkType.Slow:
					break;
			}
			currentWalk = value;
		}
	}

	public AnimationPlayer anim;
	public Sprite2D sprite;
	public RayCast2D left_ray, right_ray;

	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite2D>("Sprite2D");
		left_ray = GetNode<RayCast2D>("LeftRayCast2D");
		right_ray = GetNode<RayCast2D>("RightRayCast2D");
	}

	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed("phase"))
		{
			bool isRight = Input.IsActionPressed("right");
			bool isLeft = Input.IsActionPressed("left");
			if ((isRight && right_ray.IsColliding()) || (isLeft && left_ray.IsColliding()))
			{
				GodotObject obj = isRight ? right_ray.GetCollider() : left_ray.GetCollider();
				if (obj is IPassthrough pobj)
				{
					Position = pobj.teleportFrom(Position);
					currentWalk = WalkType.Boosted;
				}
			}
		}

		if (ev.IsActionReleased("jump"))
		{
			if (Velocity.Y < 0)
			{
				Velocity = new Vector2(Velocity.X, Velocity.Y / 2);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * (velocity.Y > 0 ? 1.3f : 1.0f) * (isDrilling ? drillGravityBoost : 1.0f);
			if (velocity.Y > MaxFallSpeed)
			{
				velocity.Y = MaxFallSpeed;
			}

			if (Input.IsActionPressed("down"))
			{
				if (!isDrilling)
				{
					/*anim.Play("attack");*/
					anim.Play("attack_loop");
				}
				isDrilling = true;
			}
			else
			{
				isDrilling = false;
			}

			if (velocity.Y > 0)
			{
				if (!isDrilling)
				{
					anim.Play("fall");
				}
			}
		}
		else
		{
			isDrilling = false;
		}

		float direction = Input.GetAxis("left", "right");
		if (direction != 0.0f)
		{
			velocity.X += direction * Speed * speedBuff;
			velocity.X = Mathf.Clamp(velocity.X, -allowedSpeed, allowedSpeed);
			velocity.X *= !IsOnFloor() ? 1.1f : 1.0f;
			sprite.FlipH = direction < 0;
			if (IsOnFloor())
			{
				if (CurrentWalk == WalkType.Normal)
				{
					anim.Play("walk");
				}
				else if (CurrentWalk == WalkType.Boosted)
				{
					anim.Play("fast_walk");
				}
			}
		}
		else
		{
			currentWalk = WalkType.Normal;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			if (velocity == Vector2.Zero)
			{
				anim.Play("idle");
			}
		}

		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump"))
			{
				velocity.Y = JumpVelocity;
				anim.Play("jump");
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void onDrillHitTarget(Node2D body)
	{
	}
}
