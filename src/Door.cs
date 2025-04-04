using Godot;
using System;
using System.Linq;

[Tool]
public partial class Door : Area2D
{
	[Signal]
	public delegate void PlayerEnteredEventHandler(String levelPath, int doorId);

	private string levelPath;

	[Export(PropertyHint.File, "*.tscn,")]
	public String LevelPath
	{
		get => levelPath;
		set
		{
			levelPath = value;
			_GetConfigurationWarnings();
		}
	}

	[Export]
	public int doorId = 0;

	public bool isVertical;

	private Vector2 enterDirection;
	[Export]
	public Vector2 EnterDirection
	{
		get => enterDirection;
		set
		{
			enterDirection = value;
			_GetConfigurationWarnings();
		}
	}

	// FIXME: this shouldn't be exported, but Godot doesn't update the value otherwise
	[Export]
	public Vector2 collisionSize;
	[Export]
	public Vector2 CollisionSize
	{
		get => collisionSize;
		set
		{
			// NOTE: this needs to be here because CollisionShape2D doesn't exist if the node isn't ready, so we postpone the setter
			if (!IsNodeReady())
			{
				Ready += () => CollisionSize = value;
				return;
			}

			collisionSize = value;
			RectangleShape2D newShape = new RectangleShape2D();
			newShape.Size = collisionSize;
			var collision = GetNode<CollisionShape2D>("CollisionShape2D");
			if (collision is not null)
			{
				collision.Shape = newShape;
			}

			_GetConfigurationWarnings();
		}
	}

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new Godot.Collections.Array<string>();

		isVertical = collisionSize.X < collisionSize.Y;

		if (!((isVertical && (enterDirection == Vector2.Right || enterDirection == Vector2.Left)) || (!isVertical && (enterDirection == Vector2.Up || enterDirection == Vector2.Down))))
		{
			warnings.Add("Direction of entry and the type of door don\'t match.");
		}

		if (levelPath is null || levelPath.Length == 0)
		{
			warnings.Add("level path cannot be empty.");
		}

		return warnings.ToArray();
	}

	public override void _Ready()
	{
		if (Engine.IsEditorHint())
		{
			return;
		}

		isVertical = collisionSize.X < collisionSize.Y;

		BodyEntered += onBodyEntered;
	}

	/**
	 * Returns the position at the border between the door and the rest of the level.
	 *
	 * If enterPosition is Up, the caller of this function should handle the way the player enters the room so that he doesn't fall back into the door.
	 */
	public Vector2 getEntryPoint()
	{
		Vector2 halfSize = collisionSize / 2;
		if (isVertical)
		{
			if (EnterDirection == Vector2.Right)
			{
				// Bottom-left
				return GlobalPosition + new Vector2(-halfSize.X, halfSize.Y);
			}

			// Bottom-right
			return GlobalPosition + halfSize;
		}

		if (EnterDirection == Vector2.Up)
		{
			// Bottom-middle
			return GlobalPosition + new Vector2(0, halfSize.Y);
		}

		// Top-middle
		return GlobalPosition + new Vector2(0, -halfSize.Y);
	}

	public void onBodyEntered(Node2D body)
	{
		if (body is Blob blob)
		{
			EmitSignal(SignalName.PlayerEntered, LevelPath, doorId);
		}
	}
}
