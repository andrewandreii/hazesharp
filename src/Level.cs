using Godot;
using System.Diagnostics;
using System.Collections.Generic;

public partial class Level : Node2D
{
	Door.PlayerEnteredEventHandler roomChangedHandler;

	List<Door> doors = new List<Door>();

	TileMapLayer basic;

	public Rect2I levelRect;

	public void init(Door.PlayerEnteredEventHandler roomChangedHandler)
	{
		this.roomChangedHandler = roomChangedHandler;
	}

	public override void _Ready()
	{
		basic = GetNode<TileMapLayer>("Basic");
		levelRect = basic.GetUsedRect();

		foreach (Node node in GetNode("Doors").GetChildren())
		{
			if (node is Door door)
			{
				doors.Add(door);
				door.PlayerEntered += roomChangedHandler;
			}
			else
			{
				GD.PushError("All nodes under Doors should inherit the class Door.");
			}
		}
	}

	public (Vector2, Vector2) getRoomEnterWalk(int doorNum, int objectWidth)
	{
		Door door = doors[doorNum];
		Vector2 entryPoint = door.getEntryPoint();

		return (entryPoint, -door.EnterDirection);
	}
}
