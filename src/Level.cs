using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

public partial class Level : Node2D
{
	Door.PlayerTransitionToEventHandler roomChangedHandler;

	List<Door> doors = new List<Door>();

	TileMapLayer basic;

	public Rect2I levelRect;

	public void init(Door.PlayerTransitionToEventHandler roomChangedHandler)
	{
		this.roomChangedHandler = roomChangedHandler;
	}

	public override void _Ready()
	{
		basic = GetNode<TileMapLayer>("Basic");
		levelRect = basic.GetUsedRect();
		GD.Print($"current level size {levelRect.Position}, {levelRect.Size}");

		foreach (Node node in GetNode("Doors").GetChildren())
		{
			if (node is Door door)
			{
				doors.Add(door);
				door.PlayerTransitionTo += roomChangedHandler;
			}
			else
			{
				Debug.Fail("All nodes under Doors should inherit the class Door.");
			}
		}
	}

	public (Vector2, Vector2) getRoomEnterWalk(int door_num, int object_width)
	{
		Door door = doors[door_num];
		Vector2 entryPoint = door.getEntryPoint();

		return (entryPoint + new Vector2(-object_width / 2, 0), entryPoint + new Vector2(object_width / 2, 0));
	}
}
