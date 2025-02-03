using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class BattleInterface : Control
{
	[Export] private Label drawLabel;
	[Export] private Label discardLabel;
	[Export] private PackedScene cardScene;
	[Export] private Button endTurnButton;
	[Export] private Label manaLabel;

	public List<CardData> playerCards {get; set;}

	private Queue<Card> drawPile = new Queue<Card>();
	private List<Card> handPile = new List<Card>();
	private Queue<Card> discardPile = new Queue<Card>();
	private BattleManager battleManager;
	private Vector2 handPosition;
	
	public override void _Ready()
	{
		battleManager = BattleManager.instance;
		LoadCards();
		UpdateLabels();
		endTurnButton.Pressed += OnEndTurn;
		handPosition = new Vector2(GameSettings.windowWidth / 2, GameSettings.windowHeight - GameSettings.cardHeight / 2);
	}
	
	public void FillHand()
	{
		if (drawPile.Count < battleManager.playerHandCapacity)
		{
			while (discardPile.Count > 0)
				drawPile.Enqueue(discardPile.Dequeue());
			
			discardPile.Clear();
		}
		
		for (int i = 0; i < battleManager.playerHandCapacity; i++)
		{
			Card card = drawPile.Dequeue();
			AddCardToHand(card);
		}
		
		UpdateLabels();
	}

	private void LoadCards()
	{
		foreach (CardData cardData in playerCards)
		{
			Card card = (Card)cardScene.Instantiate();
			card.cardData = cardData;
			card.CardConsumed += OnCardConsumed;
			
			drawPile.Enqueue(card);
		}
	}

	private void UpdateLabels()
	{
		drawLabel.Text = drawPile.Count.ToString();
		discardLabel.Text = discardPile.Count.ToString();
		manaLabel.Text = battleManager.playerMana.ToString();
	}

	private void OnCardConsumed(Card card, Entity target)
	{
		if (card.cardData.Cost > battleManager.playerMana)
		{
			Debug.Print("Can't consume this card");
			return;
		}
		
		battleManager.playerMana -= card.cardData.Cost;
		target.ApplyEffects(card, battleManager.player);
		
		RemoveCardFromHand(card);
		discardPile.Enqueue(card);
		
		UpdateLabels();
	}

	private void OnEndTurn()
	{
		for (int i = 0; i < handPile.Count; i++)
		{
			discardPile.Enqueue(handPile[0]);
			RemoveCardFromHand(handPile[0]);
		}
		
		battleManager.EndPlayerTurn();
	}

	private void AddCardToHand(Card card)
	{
		handPile.Add(card);
		AddChild(card);
		
		int startPos = (int)(-GameSettings.cardWidth * handPile.Count / 2 * 0.9);
		int spacing = (int)(GameSettings.cardWidth * 0.9);
		for (int i = 0; i < handPile.Count; i++)
		{
			handPile[i].Position = handPosition + new Vector2(startPos + i * spacing, 0);
		}
	}

	private void RemoveCardFromHand(Card card)
	{
		handPile.Remove(card);
		RemoveChild(card);
		
		int startPos = (int)(-GameSettings.cardWidth * handPile.Count / 2 * 0.9);
		int spacing = (int)(GameSettings.cardWidth * 0.9);
		for (int i = 0; i < handPile.Count; i++)
		{
			handPile[i].Position = handPosition + new Vector2(startPos + i * spacing, 0);
		}
	}
}