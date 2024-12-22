using Godot;
using System.Linq;

[Tool]
public partial class SimplePassthroughWall : StaticBody2D, IPassthrough
{
	public Vector2 size = new Vector2(10, 10);

	[Export]
	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
			CollisionShape2D shape = GetNode<CollisionShape2D>("CollisionShape2D");
			shape.Shape = new RectangleShape2D();
			((RectangleShape2D)shape.Shape).Size = size;

			var new_sprite = new Godot.Collections.Array<Vector2> { -size / 2 };
			new_sprite.Add(new Vector2(new_sprite[0].X, new_sprite[0].Y + size.Y));
			new_sprite.Add(new Vector2(new_sprite[0].X + size.X, new_sprite[0].Y + size.Y));
			new_sprite.Add(new Vector2(new_sprite[0].X + size.X, new_sprite[0].Y));

			Polygon2D sprite = GetNode<Polygon2D>("Polygon2D");
			sprite.Polygon = new_sprite.ToArray();
		}
	}

	public CollisionShape2D shape;
	public Polygon2D poly;

	public Vector2 teleportFrom(Vector2 pos)
	{
		float x = pos.X - Position.X;
		if (x < 0)
		{
			pos.X += -x + size.X + 5;
		}
		else
		{
			pos.X -= x + size.X + 5;
		}

		return pos;
	}

	public override void _Ready()
	{
		Size = size;
	}

	public override void _Process(double delta)
	{
	}
}
