using Godot;

namespace DeckbuilderPrototype.Scripts;

public partial class Hand : Control
{
	[Export] private PackedScene cardScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2 viewportSize = GetViewportRect().Size / 2;
		Position = new Vector2(viewportSize.X / 2, viewportSize.Y * 0.9f);
		
		// Create 6 cards and place them in the hand
		for (int i = 0; i < 2; i++)
		{
			Card cardInstance = (Card)cardScene.Instantiate();
			cardInstance.Position = new Vector2(50 + i * 400, Position.Y); // Set the initial position
			cardInstance.Index = i + 1;
			AddChild(cardInstance);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}