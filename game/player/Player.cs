using Godot;

namespace luja.Game.player;

public partial class Player : Entity
{
	private AnimatedSprite2D anim;
	private string dir = "down";

	protected void Play(string basename, string dir)
	{
		string animName = $"{basename}_{dir}";
		anim.Play(animName);
	}

  public override void _Ready()
  {
		Speed = 10.0f;
    anim = GetNode<AnimatedSprite2D>("animation");
  }

  public override void _PhysicsProcess(double delta)
  {
    var move = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
		Move(move, delta);

		if (move != Vector2.Zero)
		{
			dir = GetDirectionString(move);
			Play("go", dir);
		}
		else
		{
			Play("idle", dir);
		}
  }
}
