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
	[Export] private Label manaLabel;

	public List<CardData> playerCards {get; set;}

	private Queue<Card> drawPile = new Queue<Card>();
	private List<Card> handPile = new List<Card>();
	private Queue<Card> discardPile = new Queue<Card>();
	private BattleManager battleManager;
	
	public override void _Ready()
	{
		battleManager = BattleManager.instance;
		LoadCards();
		UpdateLabels();
		endTurnButton.Pressed += OnEndTurn;
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
			handPile.Add(card);
			hand.AddChild(card);
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
		
		handPile.Remove(card);
		discardPile.Enqueue(card);
		hand.RemoveChild(card);
		
		UpdateLabels();
	}

	private void OnEndTurn()
	{
		foreach (Card card in handPile)
		{
			discardPile.Enqueue(card);
			hand.RemoveChild(card);
		}
		
		handPile.Clear();
		battleManager.EndPlayerTurn();
	}
}