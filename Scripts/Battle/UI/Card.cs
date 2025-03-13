using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class Card : Control
{
	[Export] private Sprite2D image;
	[Export] private Label cardName;
	[Export] private Label cardDescription;
	[Export] private Label cardCost;
	
	[Signal] public delegate void CardConsumedEventHandler(Card card, Entity target);
	
	public CardData cardData { get; set; }

	private CardHolder cardHolder;
	private Node2D cardVisualCopy;
	public bool playerCard = true;

	public Tween tweenPos = null;
	public Tween tweenScale = null;

	private Queue<Action<Tween>> tweenPosQueue = new();
	private Queue<Action<Tween>> tweenScaleQueue = new();
	
    public override void _Ready()
    {
	    image.Texture = GD.Load<Texture2D>("res://Sprites/Cards/Effects/" + cardData.id + ".png");
	    cardName.Text = cardData.name;
	    cardDescription.Text = cardData.GetDescription();
	    cardCost.Text = cardData.cost.ToString();

	    if (playerCard)
	    {
            MouseEntered += OnMouseEnter;
            MouseExited += OnMouseExit;
            cardHolder = (CardHolder) GetParent().GetNode<Area2D>("CardHolder");
            cardVisualCopy = (Node2D) GetNode<Node2D>("CardVisual").Duplicate();
	    }
    }

    public override void _GuiInput(InputEvent @event)
    {
	    if (!playerCard)
		    return;
	    
	    if (@event is InputEventMouseButton eventMouseButton)
	    {
		    if (eventMouseButton.Pressed)
		    {
			    cardHolder.AddChild(cardVisualCopy);
			    cardVisualCopy.GlobalPosition = cardHolder.GlobalPosition + new Vector2(-GameSettings.cardWidth / 2, -GameSettings.cardHeight / 2);
			    cardHolder.Activate();
			    GetNode<Node2D>("CardVisual").Hide();
		    }
		    else
		    {
			    cardHolder.RemoveChild(cardVisualCopy);
			    cardHolder.Deactivate();
			    GetNode<Node2D>("CardVisual").Show();
			    
			    if (cardHolder.currentTarget != null)
			    {
				    // Cancel consumption if invalid target
				    if (cardHolder.currentTarget.isPlayerAlly && cardData.targetFaction == TargetFaction.Enemy)
					    return;
				    
				    if (!cardHolder.currentTarget.isPlayerAlly && cardData.targetFaction != TargetFaction.Enemy)
					    return;
				    
				    EmitSignal(SignalName.CardConsumed, this, cardHolder.currentTarget);
			    }
		    }

	    }
    }
    
    public void QueueAnimation(AnimationType type, Vector2 targetPos, float duration)
    {
	    if (type == AnimationType.Move)
	    {
	        tweenPosQueue.Enqueue(t => t.TweenProperty(this, "position", targetPos, duration));
	        
            if (tweenPos == null) 
                ProcessNextTween(type);
	    } 
	    
	    if (type == AnimationType.Scale)
	    {
		    tweenScaleQueue.Enqueue(t => t.TweenProperty(this, "scale", targetPos, duration));
		    
		    if (tweenScale == null) 
			    ProcessNextTween(type);
	    }
    }
    
    private void ProcessNextTween(AnimationType type)
    {
	    if (type == AnimationType.Move && tweenPosQueue.Count > 0)
	    {
		    tweenPos = GetTree().CreateTween();
		    var nextTask = tweenPosQueue.Dequeue();
		    nextTask(tweenPos);

		    tweenPos.Finished += () =>
		    {
			    tweenPos = null;
			    ProcessNextTween(type);
		    };
	    }
	    
	    if (type == AnimationType.Scale && tweenScaleQueue.Count > 0)
	    {
		    tweenScale = GetTree().CreateTween();
		    var nextTask = tweenScaleQueue.Dequeue();
		    nextTask(tweenScale);
		    
		    tweenScale.Finished += () =>
		    {
			    tweenScale = null;
			    ProcessNextTween(type);
		    };
	    }
    }
    
    private void OnMouseEnter()
    {
	    GetNode<AnimationPlayer>("AnimationPlayer").Play("Select");
	    ZIndex = 10;
    }
    
    private void OnMouseExit()
    {
	    GetNode<AnimationPlayer>("AnimationPlayer").Play("Deselect");
	    ZIndex = 0;
    }
    
    public enum AnimationType
    {
	    Move,
	    Scale
    }
}

