using Godot;

public partial class Blob : CharacterBody2D
{
	public enum WalkType
	{
		Normal, Boosted, Slow
	};

	[Signal]
	public delegate void healthUpdatedEventHandler(int health);
	[Signal]
	public delegate void coinsUpdatedEventHandler(int amount);

	public const float Accel = 40.0f;
	public const float JumpVelocity = -435.0f;
	public const float MaxNormalSpeed = 200.0f;
	public const float MaxSpeed = 400.0f;
	public const float MaxFallSpeed = 400.0f;
	public const float DrillGravityBoost = 1.4f;

	public bool isDrilling = false;
	public float allowedSpeed = MaxNormalSpeed;
	public float speedBuff = 1.0f;
	public bool usedDoubleJump = false;
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
	public RayCast2D leftRay, rightRay;
	public Timer iframeTimer;
	public IInteractable interactable;

	public int maxHealth = 5;
	public int health = 5;
	public uint coins = 0;
	public uint damage = 7;
	public bool hasDoubleJump = false;

	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite2D>("Sprite2D");
		leftRay = GetNode<RayCast2D>("LeftRayCast2D");
		rightRay = GetNode<RayCast2D>("RightRayCast2D");
		iframeTimer = GetNode<Timer>("IFrameTimer");
	}

	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed("phase"))
		{
			bool isRight = Input.IsActionPressed("right");
			bool isLeft = Input.IsActionPressed("left");
			if ((isRight && rightRay.IsColliding()) || (isLeft && leftRay.IsColliding()))
			{
				GodotObject obj = isRight ? rightRay.GetCollider() : leftRay.GetCollider();
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

		if (ev.IsActionPressed("interact") && interactable is not null)
		{
			interactable.use();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * (velocity.Y > 0 ? 1.3f : 1.0f) * (isDrilling ? DrillGravityBoost : 1.0f);
			if (velocity.Y > MaxFallSpeed)
			{
				velocity.Y = MaxFallSpeed;
			}

			if (Input.IsActionPressed("down"))
			{
				if (!isDrilling)
				{
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
			usedDoubleJump = false;
		}

		float direction = Input.GetAxis("left", "right");
		if (direction != 0.0f)
		{
			velocity.X += direction * Accel * speedBuff;
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
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Accel);
			if (velocity == Vector2.Zero)
			{
				anim.Play("idle");
			}
		}

		if (IsOnFloor() || (hasDoubleJump && !usedDoubleJump))
		{
			if (Input.IsActionJustPressed("jump"))
			{
				velocity.Y = JumpVelocity;
				anim.Play("jump");

				if (!IsOnFloor())
				{
					usedDoubleJump = true;
				}
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void onDrillHitTarget(Node2D body)
	{
		if (body is IEnemy enemy)
		{
			enemy.takeDamage((int)damage);
			Velocity = new Vector2(Velocity.X, JumpVelocity);
		}
	}

	public void takeDamage(int amount, bool ignoreTimer = false)
	{
		if (!iframeTimer.IsStopped() && !ignoreTimer)
		{
			return;
		}

		iframeTimer.Start();
		health -= amount;

		if (health <= 0)
		{
			Haze.World.changeRoom(Haze.worldData.levelPath, 0);
			coins = 0;
			EmitSignal(SignalName.coinsUpdated, 0);
			health = maxHealth;
		}

		EmitSignal(SignalName.healthUpdated, health);
	}

	public void addCoins(uint amount)
	{
		coins += amount;
		EmitSignal(SignalName.coinsUpdated, coins);
	}

	public bool spendMoney(uint amount)
	{
		return true;
	}

	public void onNewInteract(Area2D newInteract)
	{
		if (newInteract is IInteractable interactable)
		{
			this.interactable = interactable;
			interactable.select();
		}
	}

	public void onInteractLeft(Area2D delInteract)
	{
		if (delInteract is IInteractable interactable)
		{
			interactable.deselect();
			this.interactable = null;
		}
	}

	public void onDeathToHazard(Node2D body)
	{
		takeDamage(1, true);
		Haze.World.restartLevel();
	}
}
