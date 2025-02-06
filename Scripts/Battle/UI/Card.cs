using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
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
	    image.Texture = GD.Load<Texture2D>("res://Sprites/Cards/Effects/" + cardData.id + ".png");
	    cardName.Text = cardData.name;
	    cardDescription.Text = cardData.GetDescription();
	    cardCost.Text = cardData.cost.ToString();
	    
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
	public string id { get; set; }
	public string name { get; set; }
	public string descriptionTemplate { get; set; }
	public int cost { get; set; } = 1;
	public int tier { get; set; } = 0;
	public int progress { get; set; } = 0;
	public TargetType targetType { get; set; }
	public TargetFaction targetFaction { get; set; }
	[JsonPropertyName("effects")]
	public List<Effect> Effects { get; set; } = new();

	public string GetDescription()
	{
		return Regex.Replace(descriptionTemplate, @"\{(\w+)_(\d+)\}", match =>
		{
			string key = match.Groups[1].Value;
			int effectIndex = int.Parse(match.Groups[2].Value);
			
			Effect effect = Effects[effectIndex];
			if (key == "amount") return effect.amount.ToString();
			if (key == "duration") return effect.amount.ToString();

			// Return placeholder if no match found
			return match.Value;
		});
	}
}

public class Effect 
{
	public EffectType type { get; set; }
	public EffectSubtype subtype { get; set; }
	public FirstTriggerTiming firstTriggerTiming { get; set; }
	public DurationReductionTiming durationReductionTiming { get; set; }
	public int duration { get; set; } = 1;
	public int amount { get; set; }
	public Entity target;
	public Entity caster;

	public Effect() {}

	public Effect(EffectType type, EffectSubtype subtype, FirstTriggerTiming firstTriggerTiming, 
		DurationReductionTiming durationReductionTiming, int duration, int amount)
	{
		this.type = type;
		this.subtype = subtype;
		this.firstTriggerTiming = firstTriggerTiming;
		this.durationReductionTiming = durationReductionTiming;
		this.duration = duration;
		this.amount = amount;
	}
	
	public Effect(EffectType type, EffectSubtype subtype, FirstTriggerTiming firstTriggerTiming, 
		DurationReductionTiming durationReductionTiming, int duration, int amount, Entity target, Entity caster)
	{
		this.type = type;
		this.subtype = subtype;
		this.firstTriggerTiming = firstTriggerTiming;
		this.durationReductionTiming = durationReductionTiming;
		this.amount = amount;
		this.duration = duration;
		this.target = target;
		this.caster = caster;
	}
	
	public void ApplyEffect()
	{
		switch (type)
		{
			case EffectType.Attack:
				if (subtype is EffectSubtype.None)
				{
					int casterAttack = Math.Max(0, amount + caster.entityData.attackPower);
					int damageOnGuard = Math.Min(target.entityData.guard, casterAttack);
					target.entityData.guard -= damageOnGuard;
					target.entityData.health -= (casterAttack - damageOnGuard);
				}

				if (subtype is EffectSubtype.Bleed or EffectSubtype.Poison)
					target.entityData.health -= amount;
				
				break;
			case EffectType.Guard:
				target.entityData.guard += amount;
				break;
			case EffectType.Heal:
				target.entityData.health += amount;
				break;
			case EffectType.StatusEffect:
				if (subtype is EffectSubtype.AttackBuff)
					target.entityData.attackPower += amount;
				if (subtype is EffectSubtype.AttackDebuff)
					target.entityData.attackPower -= amount;
				if (subtype is EffectSubtype.Paralyze)
					target.entityData.isParalyzed = true;
				break;
		}
		
		if (durationReductionTiming == DurationReductionTiming.OnEffectApply)
			duration--;
	}

	public Effect Clone(Entity target, Entity caster)
	{
		return new Effect(type, subtype, firstTriggerTiming, durationReductionTiming, duration, amount, target, caster);
	}
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EffectType
{
	Attack,
	Guard,
	Heal, 
	StatusEffect
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EffectSubtype
{
	None,
	Bleed,
	Poison,
	AttackBuff,
	AttackDebuff,
	Paralyze
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TargetType
{
	Single,
	Group
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TargetFaction
{
	Self,
	Ally,
	Enemy
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FirstTriggerTiming
{
	Immediate,
	NextTurnStart
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DurationReductionTiming
{
	OnEffectApply,
	OnTurnEnd
}