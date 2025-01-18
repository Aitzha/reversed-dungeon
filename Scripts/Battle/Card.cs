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

public class CardData
{
	[JsonPropertyName("id")]
	public string Id { get; set; }
	
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; set; }
	
	[JsonPropertyName("tier")] 
	public int Tier { get; set; } = 0;

	[JsonPropertyName("progression_value")]
	public int ProgressionValue { get; set; } = 0;
	
	public List<BaseEffect> Effects { get; set; } = new();
}

public abstract class BaseEffect : IEffect
{
	public bool IsExpired { get; set; }
	public int Duration { get; set; }
	public TargetType TargetType { get; set; }
	public void Activate(string caster, string target)
	{
		throw new NotImplementedException();
	}

	public void Tick(string target)
	{
		throw new NotImplementedException();
	}
}

[JsonConverter(typeof(JsonStringEnumConverter))]
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
	void Activate(String caster, String target);
	void Tick(String target); 
}

public class InstantEffect : BaseEffect
{
	public InstantEffectType type { get; set; }
}

public class ContinuousEffect : BaseEffect
{
	public ContinuousEffectType type { get; set; }
}

public class BuffDebuffEffect : BaseEffect
{
	public BuffDebuffEffectType type { get; set; }
}

public enum InstantEffectType
{
	Attack,
	Guard,
	Heal
}

public enum ContinuousEffectType
{
	Attack,
	Guard,
	Heal
}

public enum BuffDebuffEffectType
{
	Attack,
	Guard,
	Heal
}