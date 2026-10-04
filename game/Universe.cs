using Godot;
using luja.Components;
using luja.lua;

namespace luja.Game;

public partial class Universe : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Machinery.Start();

		var provider = Function.From((object o) =>
		{
			GD.Print($"o: {o} ({o.GetType().FullName})");
		});

		Machinery.state.Push("ProvideCSharp", provider);
		if (! Machinery.Do("ProvideCSharp(System.Gift)")) throw new System.Exception(Machinery.state.ToString(-1));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
