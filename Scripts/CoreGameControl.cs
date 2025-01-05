using Godot;
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

		// Add function to the signals
		if (_pauseMenuScript != null)
		{
			_pauseMenuScript.ResumeGame += UnpauseGame;
			_pauseMenuScript.ResolutionSelected += ChangeResolution;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))  // e.g. ESC
		{
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

	private void ChangeResolution(long index)
	{
		
		switch (index)
		{
			case 0:
				DisplayServer.WindowSetSize(new Vector2I(640, 360));
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				break;
			case 1:
				DisplayServer.WindowSetSize(new Vector2I(1280, 720));
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				break;
			case 2:
				DisplayServer.WindowSetSize(new Vector2I(1920, 1080));
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
		}
	}
}
