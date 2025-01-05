using Godot;
using System;

public interface IEnemy
{
	public EnemyType Type { get; set; }

	public void takeDamage(int amount);
}
