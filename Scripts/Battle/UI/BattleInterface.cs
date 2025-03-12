using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class BattleInterface : Control
{
	[Export] private Label drawLabel;
	[Export] private Label discardLabel;
	[Export] private PackedScene cardScene;
	[Export] private Button endTurnButton;
	[Export] private Label manaLabel;

	public Array<CardData> playerCards {get; set;}

	private Queue<Card> drawPile = new();
	private List<Card> handPile = new();
	private List<Card> discardPile = new();
	private BattleManager battleManager;
	private Vector2 handPosition;

	private Sprite2D cardBack;

	public bool areCardsPickable = false;
	
	[Signal] public delegate void PlayerEndedTurnEventHandler();
	
	public override void _Ready()
	{
		battleManager = BattleManager.instance;
		LoadCards();
		UpdateLabels();
		endTurnButton.Pressed += OnEndTurn;
		handPosition = new Vector2(GameSettings.windowWidth / 2, GameSettings.windowHeight - GameSettings.cardHeight / 2);
		endTurnButton.Disabled = true;
		
		cardBack = new Sprite2D();
		cardBack.Texture = GD.Load<Texture2D>("res://Sprites/UI/Battle/card_back.png");
	}
	
	public async void FillHand()
	{
		if (drawPile.Count < battleManager.playerHandCapacity)
			await RefillDrawPile();
			
		for (int i = 0; i < battleManager.playerHandCapacity; i++)
		{
			Card card = drawPile.Dequeue();
			AddCardToHand(card);
		}
		
		UpdateLabels();
		endTurnButton.Disabled = false;
	}

	private async Task RefillDrawPile()
	{
		Shuffle(discardPile);

		// Animate card in arc shape path
		Vector2 startPos = discardLabel.GlobalPosition + new Vector2(GameSettings.cardWidth / 4, GameSettings.cardHeight / 4);
		Vector2 endPos = drawLabel.GlobalPosition + new Vector2(GameSettings.cardWidth / 4, GameSettings.cardHeight / 4);
		float midX = GameSettings.windowWidth / 2; // Midpoint X for the peak
		float peakY = startPos.Y - 30; // Arc Peak
		float a = (startPos.Y - peakY) / Mathf.Pow(startPos.X - midX, 2); // Steepness

		Tween lastTween = null;
		
		while (discardPile.Count > 0)
		{
			Card card = discardPile[0];
			discardPile.RemoveAt(0);
				
			Sprite2D cardBackSprite = (Sprite2D)cardBack.Duplicate();
			discardLabel.Text = discardPile.Count.ToString();
				
			cardBackSprite.Position = startPos;
			cardBackSprite.ZIndex = -1;
			AddChild(cardBackSprite);
			Tween tween = GetTree().CreateTween();

			tween.TweenMethod(Callable.From<float>(t =>
			{
				// Interpolate X linearly
				float x = Mathf.Lerp(startPos.X, endPos.X, t);
            
				// Calculate Y using a parabolic equation
				float y = a * Mathf.Pow(x - midX, 2) + peakY;
            
				// Apply new position
				cardBackSprite.Position = new Vector2(x, y);
			}), 0.0f, 1.0f, 0.7f);
				
			tween.TweenCallback(Callable.From(() =>
			{
				RemoveChild(cardBackSprite);
				cardBackSprite.QueueFree();
				drawPile.Enqueue(card);
				drawLabel.Text = drawPile.Count.ToString();
			}));

			lastTween = tween;
			await ToSignal(GetTree().CreateTimer(0.07f), "timeout");
		}

		if (lastTween != null)
			await ToSignal(lastTween, "finished");
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
		if (card.cardData.cost > battleManager.playerMana)
		{
			Debug.Print("Can't consume this card");
			return;
		}
		
		battleManager.playerMana -= card.cardData.cost;
		target.ApplyEffects(card, battleManager.player);
		
		RemoveCardFromHand(card);
		discardPile.Add(card);
		
		UpdateLabels();
	}

	private void OnEndTurn()
	{
		int count = handPile.Count;
		for (int i = 0; i < count; i++)
		{
			Card card = handPile[0];
			discardPile.Add(card);
			RemoveCardFromHand(card);
		}

		endTurnButton.Disabled = true;
		UpdateLabels();
		EmitSignal(SignalName.PlayerEndedTurn);
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
	
	private void Shuffle<Card>(List<Card> list)
	{
		Random rng = new Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			(list[n], list[k]) = (list[k], list[n]); // Swap elements
		}
	}
}