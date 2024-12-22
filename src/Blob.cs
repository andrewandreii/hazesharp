using Godot;
using System;

public partial class Blob : CharacterBody2D
{
	public const float Speed = 50.0f;
	public const float JumpVelocity = -400.0f;
	public const float MaxNormalSpeed = 200.0f;
	public const float MaxSpeed = 400.0f;
	public const float MaxFallSpeed = 400.0f;

	public float allowed_speed = MaxNormalSpeed;
	public float speed_buff = 1.0f;

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
					allowed_speed = MaxSpeed;
					speed_buff = 1.1f;
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
			velocity += GetGravity() * (float)delta * (velocity.Y > 0 ? 1.3f : 1.0f);
			if (velocity.Y > MaxFallSpeed)
			{
				velocity.Y = MaxFallSpeed;
			}
			if (velocity.Y > 0 && !anim.CurrentAnimation.Equals("fall"))
			{
				anim.Play("fall");
			}
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			anim.Play("jump");
			velocity.Y = JumpVelocity;
		}

		float direction = Input.GetAxis("left", "right");
		if (direction != 0.0f)
		{
			velocity.X += direction * Speed * speed_buff;
			velocity.X = Mathf.Clamp(velocity.X, -allowed_speed, allowed_speed);
			velocity.X *= !IsOnFloor() ? 1.1f : 1.0f;
			sprite.FlipH = direction < 0;
			if (!anim.CurrentAnimation.Equals("walk") && IsOnFloor() && velocity.Y == 0)
			{
				if (Math.Abs(velocity.X) > MaxNormalSpeed)
				{
					anim.Play("fast_walk");
				}
				else
				{
					anim.Play("walk");
				}
			}
		}
		else
		{
			allowed_speed = MaxNormalSpeed;
			speed_buff = 1.0f;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			if (!anim.CurrentAnimation.Equals("idle") && IsOnFloor() && velocity.Y == 0)
			{
				anim.Play("idle");
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
