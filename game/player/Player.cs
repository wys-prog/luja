using Godot;

namespace luja.Game.player;

public partial class Player : Entity
{
	private AnimatedSprite2D anim;

	protected void Play(string basename)
	{
		string animName = $"{basename}_{direction}";
		anim.Play(animName);
	}

  public override void _Ready()
  {
    anim = GetNode<AnimatedSprite2D>("animation");
		
  }
}
