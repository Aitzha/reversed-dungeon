using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
	
    public override void _Ready()
    {
	    image.Texture = GD.Load<Texture2D>("res://Sprites/Cards/Effects/" + cardData.Id + ".png");
	    cardName.Text = cardData.Name;
	    cardDescription.Text = cardData.GetDescription();
	    cardCost.Text = cardData.Cost.ToString();
	    
        MouseEntered += OnMouseEnter;
        MouseExited += OnMouseExit;

        cardHolder = (CardHolder) GetParent().GetNode<Area2D>("CardHolder");
        cardVisualCopy = (Node2D) GetNode<Node2D>("CardVisual").Duplicate();
    }

    public override void _GuiInput(InputEvent @event)
    {
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
				    EmitSignal(SignalName.CardConsumed, this, cardHolder.currentTarget);
			    }
		    }

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
}

public class CardData
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string DescriptionTemplate { get; set; }
	public int Cost { get; set; } = 1;
	public int Tier { get; set; } = 0;
	public int ProgressionValue { get; set; } = 0;
	
	public List<BaseEffect> Effects { get; set; } = new();

	public string GetDescription()
	{
		return Regex.Replace(DescriptionTemplate, @"\{(\w+)_(\d+)\}", match =>
		{
			string key = match.Groups[1].Value;
			int effectIndex = int.Parse(match.Groups[2].Value);
			
			BaseEffect effect = Effects[effectIndex];
			if (key == "amount") return effect.Amount.ToString();
			if (key == "duration") return effect.Duration.ToString();

			// Return placeholder if no match found
			return match.Value;
		});
	}
}

public abstract class BaseEffect : IEffect
{
	public string EffectType { get; set; }
	public bool IsExpired { get; set; }
	public int Amount { get; set; }
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

	public InstantEffect()
	{
		EffectType = "InstantEffect";
	}

	public InstantEffect(InstantEffectSubType subType, int amount)
	{
		EffectType = "InstantEffect";
		SubType = subType;
		Amount = amount;
	}

	public override void Activate(Entity caster, Entity target)
	{
		switch (SubType)
		{
			case InstantEffectSubType.Attack:
				int casterAttack = Math.Max(0, Amount + caster.entityData.attackPower);
				int damageOnGuard = Math.Min(target.entityData.guard, casterAttack);
				target.entityData.guard -= damageOnGuard;
				target.entityData.health -= (casterAttack - damageOnGuard);
				break;
			case InstantEffectSubType.Guard:
				target.entityData.guard += Amount;
				break;
			case InstantEffectSubType.Heal:
				target.entityData.health = Math.Min(target.entityData.health + Amount, target.entityData.maxHealth);
				break;
		}
	}

	public override void Tick(Entity target)
	{
		// Typically nothing here for an instant effect
	}
}

public class ContinuousEffect : BaseEffect
{
	public ContinuousEffectSubType SubType { get; set; }

	public ContinuousEffect()
	{
		EffectType = "ContinuousEffect";
	}

	public override void Activate(Entity caster, Entity target)
	{
		// If you need an initial effect
	}

	public override void Tick(Entity target)
	{
		switch (SubType)
		{
			case ContinuousEffectSubType.Bleed:
				target.entityData.health -= Amount;
				Duration--;
				break;
		}
		
		if (Duration <= 0)
			IsExpired = true;
	}
}

public class BuffDebuffEffect : BaseEffect
{
	public BuffDebuffEffectSubType SubType { get; set; }

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
		switch (SubType)
		{
			case BuffDebuffEffectSubType.AttackBuff:
				target.entityData.attackPower += Amount;
				Duration--;
				break;
			case BuffDebuffEffectSubType.AttackDebuff:
				target.entityData.attackPower -= Amount;
				Duration--;
				break;
		}
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