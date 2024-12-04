using System;
using Godot;

namespace DeckbuilderPrototype.Scripts;

public partial class Card : Area2D
{
	private Vector2 originalPosition;
	private Vector2 originalLPosition;
	private bool isBeingDragged = false;
	public int index = 0;
	
	[Export] private CollisionShape2D collisionShape2D;
	private Rect2 areaRect;
	private bool mouseOver = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		originalPosition = GlobalPosition;
		originalLPosition = Position;
		areaRect = collisionShape2D.GetShape().GetRect();
		areaRect.Position += GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isBeingDragged)
		{
			// While dragging, update the position to the mouse position
			Position = GetGlobalMousePosition() - originalLPosition;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotionEvent)
		{
			if (areaRect.HasPoint(mouseMotionEvent.Position))
			{
				mouseOver = true;
				GD.Print("Mouse over: " + index);
			}
			else
			{
				mouseOver = false;
			}
		}
		
		// Check if mouse was pressed
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
			{
				if (mouseButtonEvent.Pressed && mouseOver)
				{
					// Start dragging when the left mouse button is pressed
					isBeingDragged = true;
					GD.Print("Dragging: " + index);
				}
				else
				{
					// Stop dragging and reset the card's position
					isBeingDragged = false;
					GlobalPosition = originalPosition; // Return to original position
				}
			}
		}
		
	}
}