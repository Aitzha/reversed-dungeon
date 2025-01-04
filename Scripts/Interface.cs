using Godot;

public partial class Interface : Control
{
	[Export] private Label _drawLabel;
	[Export] private Label _discardLabel;
	[Export] private PackedScene _cardScene;
	[Export] private Control _handControl;

	private int _cardsOnDrawPile = 100;
	private int _cardsOnHand = 0;
	private int _cardsOnDiscardPile = 0;
	
	
	public override void _Ready()
	{
		UpdateLabels();
	}
	
	public override void _Process(double delta)
	{
		// Check if the "jump" action (spacebar) was just pressed
		if (Input.IsActionJustPressed("jump"))
		{
			UpdateCardsCount();
			
			RefreshCardsOnHand();
			
			UpdateLabels();
		}
	}

	private void UpdateCardsCount()
	{
		_cardsOnDiscardPile += _cardsOnHand;
		_cardsOnDrawPile -= 2;
		_cardsOnHand = 2;
	}

	private void RefreshCardsOnHand()
	{
		foreach (Node child in _handControl.GetChildren())
		{
			// Check if the child is the instance of the card type
			if (child is CardInteraction card)
			{
				_handControl.RemoveChild(card);
				child.QueueFree();
			}
		}
		
		// Create card and add them to the hand
		for (int i = 0; i < _cardsOnHand; i++)
		{
			CardInteraction cardInteractionInstance = (CardInteraction)_cardScene.Instantiate();
			cardInteractionInstance.Position = new Vector2(50 + i * 400, _handControl.Position.Y); // Set the initial position
			cardInteractionInstance.Index = i + 1;
			_handControl.AddChild(cardInteractionInstance);
			
			// Add function to the signal
			cardInteractionInstance.CardConsumed += ConsumeCard;
		}
	}

	private void UpdateLabels()
	{
		_drawLabel.Text = _cardsOnDrawPile.ToString();
		_discardLabel.Text = _cardsOnDiscardPile.ToString();
	}

	private void ConsumeCard(int index, CardInteraction cardInteractionInstance)
	{
		_cardsOnHand--;
		_cardsOnDiscardPile++;
		
		_handControl.RemoveChild(cardInteractionInstance);
		cardInteractionInstance.CardConsumed -= ConsumeCard;
		cardInteractionInstance.QueueFree();
		
		UpdateLabels();
		GD.Print($"Card Removed by Index: {index}");
	}
}