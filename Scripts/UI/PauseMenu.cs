using Godot;

public partial class PauseMenu : Control
{
	[Export] public Button ResumeButton;
	[Export] public OptionButton ResolutionButton;
	[Signal] public delegate void ResumeGameEventHandler();
	[Signal] public delegate void ResolutionSelectedEventHandler(long index);
	
	public override void _Ready()
	{
		ResumeButton.Pressed += OnResumeButtonPressed;
		ResolutionButton.ItemSelected += OnResolutionSelected;
	}

	private void OnResumeButtonPressed()
	{
		EmitSignal(SignalName.ResumeGame);
	}

	private void OnResolutionSelected(long index)
	{
		EmitSignal(SignalName.ResolutionSelected, index);
	}
}
