using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class BattleManager : Control
{
	[Export] private Label drawLabel;
	[Export] private Label _discardLabel;
	[Export] private PackedScene _cardScene;
	
	private Vector2 _handPosition;

	private Queue<Card> _drawPile = new Queue<Card>();
	private List<Card> _handPile = new List<Card>();
	private Queue<Card> _discardPile = new Queue<Card>();
	
	public override void _Ready()
	{
		LoadCards();
		
		UpdateLabels();
		
		Vector2 viewportSize = GetViewportRect().Size / 2;
		_handPosition = new Vector2(viewportSize.X, viewportSize.Y);
		Debug.Print(_handPosition.ToString());
	}
	
	public override void _Process(double delta)
	{
		// Check if the "jump" action (spacebar) was just pressed
		if (Input.IsActionJustPressed("jump"))
		{
			RefreshCardsOnHand();
			
			UpdateLabels();
		}
	}

	private void LoadCards()
	{
		// Just random number I chose
		for (int i = 0; i < 10; i++)
		{
			CardStats cardStats = new CardStats();
			Card card = (Card)_cardScene.Instantiate();
			card.CardStats = cardStats;
			AddChild(card);
			card.HideCard();
			card.CardConsumed += ConsumeCard;
			
			_drawPile.Enqueue(card);
		}
	}

	private void RefreshCardsOnHand()
	{
		foreach (Card card in _handPile)
		{
			_discardPile.Enqueue(card);
			card.HideCard();
		}
		
		_handPile.Clear();
		
		if (_drawPile.Count <= 0)
		{
			Debug.Print("Draw pile is empty");
			return;
		}
		
		for (int i = 0; i < Math.Min(2, _drawPile.Count); i++)
		{
			Card card = _drawPile.Dequeue();
			_handPile.Add(card);
			card.Position = new Vector2(_handPosition.X - 50 + i * 100, _handPosition.Y + 100);
			card.ShowCard();
		}
	}

	private void UpdateLabels()
	{
		drawLabel.Text = _drawPile.Count.ToString();
		_discardLabel.Text = _discardPile.Count.ToString();
	}

	private void ConsumeCard(Card card)
	{
		_handPile.Remove(card);
		_discardPile.Enqueue(card);
		card.HideCard();
		
		UpdateLabels();
	}
}