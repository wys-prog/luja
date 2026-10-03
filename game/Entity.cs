using Godot;

namespace luja.Game;

[GlobalClass]
public partial class Entity : CharacterBody2D
{
	public static string GetDirectionString(Vector2 vec)
	{
		if (Mathf.Abs(vec.X) > Mathf.Abs(vec.Y))
			return vec.X > 0 ? "right" : "left";

		return vec.Y > 0 ? "down" : "up";
	}

	public float Speed { get; protected set; } = 20.0f;

	public void Move(Vector2 dir, double delta)
	{
		float actualSpeed = Speed * 250 * (float)delta;
		Velocity = dir * new Vector2(actualSpeed, actualSpeed);

		MoveAndSlide();
	}
}
