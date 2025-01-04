using Godot;
using System;
using System.Diagnostics;

public partial class CoreGameControl : Node2D
{
	[Export] public PackedScene PauseMenuScene;
	private Control _pauseMenu;
	private PauseMenuScript _pauseMenuScript;
	
	public override void _Ready()
	{
		// Instance the pause menu, script and add to Main
		_pauseMenu = (Control)PauseMenuScene.Instantiate();
		_pauseMenuScript = _pauseMenu as PauseMenuScript;
		AddChild(_pauseMenu);
		
		// Hide it initially
		_pauseMenu.Visible = false;
		
		// Change process mode to "When Paused"
		_pauseMenu.ProcessMode = ProcessModeEnum.WhenPaused;

		// Add function to the signal
		_pauseMenuScript.ResumeGame += UnpauseGame;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))  // e.g. ESC
		{
			Debug.Print("UI cancelled");
			if (GetTree().Paused)
				UnpauseGame();
			else
				PauseGame();
		}
	}

	private void PauseGame()
	{
		_pauseMenu.Visible = true;
		GetTree().Paused = true;
	}

	private void UnpauseGame()
	{
		GetTree().Paused = false;
		_pauseMenu.Visible = false;
	}
}
