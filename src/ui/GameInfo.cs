using Godot;
using System;

public partial class GameInfo : Label
{
	public Timer timer;
	public int lettersWritten = 0;
	public String buffer;
	public float fadeTimer = 4;
	public bool isWriting = false;
	public double letterTimeInterval = 0.01;
	public double timeSinceLastLetter = 0;

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Timeout += () => Text = "";
		Text = "";
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		timeSinceLastLetter += delta;
		while (timeSinceLastLetter >= letterTimeInterval)
		{
			timeSinceLastLetter -= letterTimeInterval;
			writeNewLetter();
		}
	}

	public void showInfo(String info)
	{
		if (isWriting)
		{
			return;
		}

		timer.Stop();
		Text = "";
		buffer = info;
		isWriting = true;
		lettersWritten = 0;
		SetProcess(true);
	}

	public void writeNewLetter()
	{
		if (buffer.Length <= lettersWritten)
		{
			SetProcess(false);
			isWriting = false;
			timer.Start();
			return;
		}

		Text += buffer[lettersWritten];
		++lettersWritten;
	}
}
