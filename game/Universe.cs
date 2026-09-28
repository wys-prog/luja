using Godot;
using luja.Components;

namespace luja.Game;

public partial class Universe : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Machinery.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
