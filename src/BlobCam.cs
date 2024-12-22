using Godot;
using System;

public partial class BlobCam : Node2D
{

	[Export]
	public Blob mrBlob;

	public Vector2 offset;

	public Vector2 viewportSize;

	public Vector2 minLimit = new Vector2(-10000000, -10000000);

	public Vector2 maxLimit = new Vector2(10000000, 10000000);

	public Vector2 MinLimit
	{
		get => minLimit;
		set
		{
			minLimit = value;
		}
	}

	public Vector2 MaxLimit
	{
		get => maxLimit + viewportSize;
		set
		{
			maxLimit = value - viewportSize;
		}
	}

	public override void _Ready()
	{
		viewportSize = GetViewportRect().Size;
	}

	public override void _Process(double delta)
	{
		Vector2 vel = mrBlob.Velocity;
		if (vel.Y < 0)
		{
			vel.Y = 0;
		}
		Vector2 inFront = vel.Normalized() * (float)(Math.Pow(vel.Length() / Blob.MaxSpeed, 3) * 15);

		Vector2 pos;

		/*Position = Position.Lerp(mrBlob.Position + inFront, 0.8f);*/
		pos = mrBlob.Position.Round();
		offset = offset.Lerp(inFront, 0.5f) + new Vector2(0, -16);

		Vector2 viewportTranslate = pos + offset - viewportSize / 2;
		viewportTranslate = viewportTranslate.Clamp(minLimit, maxLimit);
		GD.Print(viewportTranslate, minLimit, maxLimit);

		Transform2D canvasTransform = Transform2D.Identity;
		canvasTransform = canvasTransform.Translated(-viewportTranslate);
		GetViewport().CanvasTransform = canvasTransform;
	}
}
