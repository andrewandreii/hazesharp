using Godot;
using System;

public partial class BlobCam : Camera2D
{

	[Export]
	public Blob mrBlob;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Vector2 vel = mrBlob.Velocity;
		if (vel.Y < 0)
		{
			vel.Y = 0;
		}
		Vector2 inFront = vel.Normalized() * (float)(Math.Pow(vel.Length() / Blob.MaxSpeed, 3) * 15);
		/*Position = Position.Lerp(mrBlob.Position + inFront, 0.8f);*/
		Position = mrBlob.Position.Round();
		Offset = Offset.Lerp(inFront, 0.5f) + new Vector2(0, -16);
	}
}
