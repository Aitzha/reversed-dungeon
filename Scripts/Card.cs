using Godot;

namespace DeckbuilderPrototype.Scripts;

public partial class Card : Area2D
{
	private Vector2 _originalGlobalPos;
	private bool _isBeingDragged;
	public int Index = 0;
	
	[Signal] public delegate void CardConsumedEventHandler(int index, Card card);
	
	public override void _Ready()
	{
		_isBeingDragged = false;
		_originalGlobalPos = GlobalPosition;
		InputEvent += OnCardClicked;
	}
	
	private void OnCardClicked(Node viewport, InputEvent inputEvent, long shapeIdx)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				// Start dragging
				_isBeingDragged = true;
				GD.Print("Mouse Pressed - Start Dragging");
			}
			else
			{
				// End dragging
				if (_isBeingDragged)
				{
					_isBeingDragged = false;
					float distance = (GlobalPosition - _originalGlobalPos).Length();
					if (distance > 300.0f)
						EmitSignal(SignalName.CardConsumed, Index, this);
					GlobalPosition = _originalGlobalPos;
					GD.Print("Mouse Released - Stop Dragging");
				}
			}
		}
		
		// Detect mouse motion
		if (_isBeingDragged && inputEvent is InputEventMouseMotion mouseMotionEvent)
		{
			GlobalPosition = mouseMotionEvent.GlobalPosition;
		}
	}
}