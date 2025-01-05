using Godot;
using System;
using System.Collections.Generic;

public partial class GameProgression : Resource
{
	public static Godot.Collections.Array Bosses;

	public static Godot.Collections.Dictionary LevelsSeen = new Godot.Collections.Dictionary();

	public void storeInFile(FileAccess file)
	{
		var buffer = GD.VarToBytes(Bosses);
		file.Store32((uint)buffer.Length);
		file.StoreBuffer(buffer);
		buffer = GD.VarToBytes(LevelsSeen);
		file.Store32((uint)buffer.Length);
		file.StoreBuffer(buffer);
	}

	public void loadFromFile(FileAccess file)
	{
		int bufferLength = (int)file.Get32();
		Bosses = GD.BytesToVar(file.GetBuffer(bufferLength)).As<Godot.Collections.Array>();
		bufferLength = (int)file.Get32();
		LevelsSeen = GD.BytesToVar(file.GetBuffer(bufferLength)).As<Godot.Collections.Dictionary>();
	}
}
