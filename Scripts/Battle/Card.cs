using Godot;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Card : Control
{
	[Export] public Sprite2D image;
	[Export] public Label cardName;
	[Export] public Label cardDescription;
	
	public CardData cardData { get; set; }

	private CardHolder cardHolder;
	private Node2D cardVisualCopy;
	
    public override void _Ready()
    {
	    image.Texture = GD.Load<Texture2D>("res://Sprites/Cards/" + cardData.Id + ".png");
	    cardName.Text = cardData.Name;
	    cardDescription.Text = cardData.Description;
	    
        MouseEntered += OnMouseEnter;
        MouseExited += OnMouseExit;

        cardHolder = (CardHolder) GetParent().GetParent().GetNode<Area2D>("CardHolder");
        cardVisualCopy = (Node2D) GetNode<Node2D>("CardVisual").Duplicate();
    }
    
    private void OnMouseEnter()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Select");
    }
    
    private void OnMouseExit()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Deselect");
    }

    public override void _GuiInput(InputEvent @event)
    {
	    if (@event is InputEventMouseButton eventMouseButton)
	    {
		    if (eventMouseButton.Pressed)
		    {
			    cardHolder.AddChild(cardVisualCopy);
			    cardVisualCopy.GlobalPosition = cardHolder.GlobalPosition + new Vector2(-Globals.cardWidth / 2, -Globals.cardHeight / 2);
			    cardHolder.Activate();
			    GetNode<Node2D>("CardVisual").Hide();
		    }
		    else
		    {
			    if (cardHolder.overTarget)
			    {
				    Debug.Print("Card was consumed");
			    }
			    cardHolder.RemoveChild(cardVisualCopy);
			    cardHolder.Deactivate();
			    GetNode<Node2D>("CardVisual").Show();
		    }

	    }
    }
}

public class CardData
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int Tier { get; set; } = 0;
	public int ProgressionValue { get; set; } = 0;
	
	public List<BaseEffect> Effects { get; set; } = new();
}

public abstract class BaseEffect : IEffect
{
	public string EffectType { get; set; }
	public bool IsExpired { get; set; }
	public int Duration { get; set; }
	public TargetType TargetType { get; set; }
	public abstract void Activate(Entity caster, Entity target);
	public abstract void Tick(Entity target);
}

public enum TargetType
{
	Player,
	Enemy,
	EnemyGroup
}

public interface IEffect
{
	bool IsExpired { get; set; }
	int Duration { get; set; }
	TargetType TargetType { get; set; }
}

public class InstantEffect : BaseEffect
{
	public InstantEffectSubType SubType { get; set; }
	public int Amount { get; set; }

	public InstantEffect()
	{
		this.EffectType = "InstantEffect";
	}

	public override void Activate(Entity caster, Entity target)
	{
		// Attack, Guard, Heal logic ...
		IsExpired = true;
	}

	public override void Tick(Entity target)
	{
		// Typically nothing here for an instant effect
	}
}

public class ContinuousEffect : BaseEffect
{
	public ContinuousEffectSubType SubType { get; set; }
	public int Amount { get; set; }

	public ContinuousEffect()
	{
		this.EffectType = "ContinuousEffect";
	}

	public override void Activate(Entity caster, Entity target)
	{
		// If you need an initial effect
	}

	public override void Tick(Entity target)
	{
		// Poison/Bleed damage, etc.
		// Decrement Duration, set IsExpired if done
	}
}

public class BuffDebuffEffect : BaseEffect
{
	public BuffDebuffEffectSubType SubType { get; set; }
	public int Amount { get; set; }

	public BuffDebuffEffect()
	{
		this.EffectType = "BuffDebuffEffect";
	}

	public override void Activate(Entity caster, Entity target)
	{
		// Immediately apply buff/debuff
	}

	public override void Tick(Entity target)
	{
		// If it has a duration, decrement & revert if expired
	}
}

public enum InstantEffectSubType
{
	Attack,
	Guard,
	Heal
}

public enum ContinuousEffectSubType
{
	Poison,
	Bleed,
	Paralyze,
	Regeneration
}

public enum BuffDebuffEffectSubType
{
	AttackBuff,
	AttackDebuff,
	GuardBuff,
	GuardDebuff,
	CardCostDecrease
}