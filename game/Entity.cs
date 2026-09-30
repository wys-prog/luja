using Godot;

namespace luja.Game;

[GlobalClass]
public partial class Entity : CharacterBody2D
{
	public enum Direction
	{
		up,
		down,
		left,
		right
	}

	protected Direction direction;
}
