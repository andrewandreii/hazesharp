using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class Door : Area2D
{
	[Signal]
	public delegate void PlayerTransitionToEventHandler(String levelPath, int doorId);

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

		if (!forceIsVertical)
		{
			isVertical = collisionSize.X < collisionSize.Y;
		}
		else
		{
			isVertical = forceIsVertical;
		}

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
			EmitSignal(SignalName.PlayerTransitionTo, LevelPath, doorId);
		}
		else
		{
			// TODO: free entities that enter the door (level should manage freeing entities most likely)
		}
	}
}
