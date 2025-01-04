using Godot;

public partial class PauseMenuScript : Control
{
	[Export] public Button ResumeButton;
	[Signal] public delegate void ResumeGameEventHandler();
	
	public override void _Ready()
	{
		ResumeButton.Pressed += OnResumeButtonPressed;
	}

	private void OnResumeButtonPressed()
	{
		EmitSignal(SignalName.ResumeGame);
	}
}
