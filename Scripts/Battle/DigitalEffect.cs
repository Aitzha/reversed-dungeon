using Godot;

public partial class DigitalEffect : Node2D
{
    [Export] private AnimatedSprite2D visualEffect;
    [Export] private AudioStreamPlayer2D audioEffect;

    public override void _Ready()
    {
        visualEffect.Play();
        audioEffect.Play();
    }

    public async void Play(Entity target)
    {
        target.AddChild(this);
        GlobalPosition = target.GetGlobalPosition();
        visualEffect.Play();
        audioEffect.Play();
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        target.RemoveChild(this);
        QueueFree();
    }
}
