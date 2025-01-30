using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class BattleInterface : Control
{
	[Export] private Label drawLabel;
	[Export] private Label discardLabel;
	[Export] private PackedScene cardScene;
	[Export] private HBoxContainer hand;
	[Export] private Button endTurnButton;

	public List<CardData> playerCards {get; set;}

	private Queue<Card> drawPile = new Queue<Card>();
	private List<Card> handPile = new List<Card>();
	private Queue<Card> discardPile = new Queue<Card>();
	
	public override void _Ready()
	{
		LoadCards();
		UpdateLabels();
		endTurnButton.Pressed += BattleEventBus.instance.OnEndTurn;
		endTurnButton.Pressed += OnEndTurn;
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("jump"))
		{
			RefreshCardsOnHand();
			UpdateLabels();
		}
	}

	private void LoadCards()
	{
		foreach (CardData cardData in playerCards)
		{
			Card card = (Card)cardScene.Instantiate();
			card.cardData = cardData;
			
			drawPile.Enqueue(card);
		}
	}

	private void RefreshCardsOnHand()
	{
		OnEndTurn();
		
		if (drawPile.Count <= 0)
		{
			Debug.Print("Draw pile is empty");
			return;
		}

		int cardCount = Math.Min(2, drawPile.Count);
		
		for (int i = 0; i < cardCount; i++)
		{
			Card card = drawPile.Dequeue();
			handPile.Add(card);
			hand.AddChild(card);
		}
	}

	private void UpdateLabels()
	{
		drawLabel.Text = drawPile.Count.ToString();
		discardLabel.Text = discardPile.Count.ToString();
	}

	private void ConsumeCard(Card card)
	{
		// _handPile.Remove(oldCard);
		// _discardPile.Enqueue(oldCard);
		// oldCard.HideCard();
		//
		// UpdateLabels();
	}

	private void OnEndTurn()
	{
		foreach (Card card in handPile)
		{
			discardPile.Enqueue(card);
			hand.RemoveChild(card);
		}
		
		handPile.Clear();
	}
}