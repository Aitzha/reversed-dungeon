using Godot;

namespace DeckbuilderPrototype.Scripts;

public partial class Hand : Node2D
{
	[Export] private PackedScene cardScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2 viewportSize = GetViewportRect().Size / 2;
		Position = new Vector2(viewportSize.X / 2, viewportSize.Y * 0.9f);
		
		// Create 6 cards and place them in the hand
		for (int i = 0; i < 3; i++)
		{
			StaticBody2D cardInstance = (StaticBody2D)cardScene.Instantiate();
			cardInstance.Position = new Vector2(150 + i * 120, Position.Y); // Set the initial position
			AddChild(cardInstance);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}