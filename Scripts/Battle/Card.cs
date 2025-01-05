using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class Card : Area2D
{
	public CardStats CardStats { get; set; }
	
	private Vector2 _originalGlobalPos;
	private bool _isBeingDragged;
	private Node2D _enteredBody;
	
	[Signal] public delegate void CardConsumedEventHandler(Card card);
	
	public override void _Ready()
	{
		_isBeingDragged = false;
		InputEvent += OnCardClicked;
		BodyEntered += OnBodyEnter;
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
		_originalGlobalPos = new Vector2(-100, -100);
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
		_enteredBody = body;
	}

	private void OnBodyExited(Node2D body)
	{
		_enteredBody = null;
	}
}

public class CardStats
{
	public int Id;
	public String Name;
	public String Description;
	public int Tier;
	public int ProgressionValue;
	public List<Effect> Effects;

	public CardStats()
	{
		Id = 0;
		Name = "";
		Description = "";
		Tier = 0;
		ProgressionValue = 0;
		Effects = new List<Effect>();
	}
	
	public CardStats(int id, String name, String description, int tier, List<Effect> effects)
	{
		
	}
}

public class Effect
{
	public Target Target;
	public TargetType TargetType;
	public EffectType EffectType;
	public int Amount;
	public int Length;

}

public enum Target
{
	Player, 
	Enemy
}

public enum TargetType
{
	Single,
	Multi
}

public enum EffectType
{
	Heal,
	Guard,
	Attack
}