using Godot;

namespace DeckbuilderPrototype.Scripts;

public partial class Hand : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2 viewportSize = GetViewportRect().Size / 2;
		Position = new Vector2(viewportSize.X / 2, viewportSize.Y * 0.9f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}