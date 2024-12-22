using Godot;
using System;
using System.Diagnostics;

[Tool]
public partial class Door : Area2D
{
	[Signal]
	public delegate void PlayerTransitionToEventHandler(String levelName, int doorId);

	[Export(PropertyHint.File, "*.tscn,")]
	public String levelName;
	[Export]
	public int doorId;

	[Export]
	public bool ForceIsVertical
	{
		get => forceIsVertical;
		set
		{
			forceIsVertical = value;
			NotifyPropertyListChanged();
		}
	}

	public bool forceIsVertical = false;
	public bool isVertical;

	[Export]
	public Vector2 enterDirection;

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
		}
	}

	/*public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()*/
	/*{*/
	/*	if (Engine.IsEditorHint())*/
	/*	{*/
	/*		var props = new Godot.Collections.Array<Godot.Collections.Dictionary>();*/
	/**/
	/*		if (forceIsVertical)*/
	/*		{*/
	/*			props.Add(new Godot.Collections.Dictionary()*/
	/*		{*/
	/*			{ "name", "is_vertical" },*/
	/*			{ "type", (int)Variant.Type.Bool },*/
	/*		});*/
	/*		}*/
	/**/
	/*		return props;*/
	/*	}*/
	/**/
	/*	return null;*/
	/*}*/
	/**/
	/*public override Variant _Get(StringName property)*/
	/*{*/
	/*	if (property.ToString() == "is_vertical")*/
	/*	{*/
	/*		return isVertical;*/
	/*	}*/
	/**/
	/*	return default;*/
	/*}*/
	/**/
	/*public override bool _Set(StringName property, Variant value)*/
	/*{*/
	/*	if (property.ToString() == "is_vertical")*/
	/*	{*/
	/*		isVertical = value.AsBool();*/
	/*		return true;*/
	/*	}*/
	/**/
	/*	return false;*/
	/*}*/

	public override void _Ready()
	{
		if (Engine.IsEditorHint())
		{
			return;
		}

		if (!forceIsVertical)
		{
			isVertical = collisionSize.X < collisionSize.Y;
		}
		else
		{
			isVertical = forceIsVertical;
		}

		Debug.Assert((isVertical && (enterDirection == Vector2.Right || enterDirection == Vector2.Left)) || (!isVertical || (enterDirection == Vector2.Up || enterDirection == Vector2.Down)));

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
			if (enterDirection == Vector2.Right)
			{
				// Bottom-right
				return GlobalPosition + halfSize;
			}

			// Bottom-left
			return GlobalPosition + new Vector2(-halfSize.X, halfSize.Y);
		}

		if (enterDirection == Vector2.Up)
		{
			// Top-middle
			return GlobalPosition + new Vector2(-halfSize.X, 0);
		}

		// Bottom-middle
		return GlobalPosition + new Vector2(halfSize.X, 0);
	}

	public void onBodyEntered(Node2D body)
	{
		GD.Print($"door triggered at {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} body is at {body.Position}");
		GD.Print($"\tdoor is at {Position}");
		GD.Print($"\tdetected by {Name}");
		if (body is Blob blob)
		{
			EmitSignal(SignalName.PlayerTransitionTo, levelName, doorId);
		}
		else
		{
			// TODO: free entities that enter the door (level should manage freeing entities most likely)
		}
	}
}
