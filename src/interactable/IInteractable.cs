using Godot;
using System;

public interface IInteractable
{
	public void select();
	public void deselect();
	public void use();
}
