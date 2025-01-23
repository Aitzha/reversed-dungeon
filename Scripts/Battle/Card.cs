using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Godot;

public partial class Card : Area2D
{
	[Export] public Sprite2D image;
	[Export] public Label cardName;
	[Export] public Label cardDescription;
	
	public CardData CardData { get; set; }
	
	private Vector2 _originalGlobalPos;
	private bool _isBeingDragged;
	private Node2D _enteredBody;
	
	[Signal] public delegate void CardConsumedEventHandler(Card card);

	public void Setup()
	{
		image.Texture = GD.Load<Texture2D>("res://Sprites/Cards/" + CardData.Id + ".png");
		cardName.Text = CardData.Name;
		cardDescription.Text = CardData.Description;
	}
	
	public override void _Ready()
	{
		Setup();
		_isBeingDragged = false;
		InputEvent += OnCardClicked;
		BodyEntered += OnBodyEnter;
		BodyExited += OnBodyExited;
	}

	public void ShowCard()
	{
		Visible = true;
		DisableMode = DisableModeEnum.Remove;
		_originalGlobalPos = GlobalPosition;
	}

	public void HideCard()
	{
		Visible = false;
		DisableMode = DisableModeEnum.KeepActive;
		_originalGlobalPos = new Vector2(-200, -200);
		Position = _originalGlobalPos;
	}
	
	private void OnCardClicked(Node viewport, InputEvent inputEvent, long shapeIdx)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				// Start dragging
				_isBeingDragged = true;
			}
			else
			{
				// End dragging
				if (_isBeingDragged)
				{
					_isBeingDragged = false;
					if (_enteredBody != null)
					{
						EmitSignal(SignalName.CardConsumed, this);
						Debug.Print("Card consumed by: " + _enteredBody.Name);
					}
						
					GlobalPosition = _originalGlobalPos;
				}
			}
		}
		
		// Detect mouse motion
		if (_isBeingDragged && inputEvent is InputEventMouseMotion mouseMotionEvent)
		{
			GlobalPosition = mouseMotionEvent.GlobalPosition;
		}
	}

	private void OnBodyEnter(Node2D body)
	{
		Debug.Print("Entered body: " + body.Name);
		Entity entity = body as Entity;
		if (entity != null)
		{
			entity.ToggleGlow();
			_enteredBody = body;
		}
	}

	private void OnBodyExited(Node2D body)
	{
		Debug.Print("Exited body: " + body.Name);
		Entity entity = _enteredBody as Entity;
		if (entity != null)
		{
			entity.ToggleGlow();
			_enteredBody = null;
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