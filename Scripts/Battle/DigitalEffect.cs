using System.Diagnostics;
using Godot;

public partial class DigitalEffect : Node2D
{
    [Export] private AnimatedSprite2D visualEffect;
    [Export] private AudioStreamPlayer2D audioEffect;

    private RandomNumberGenerator rand = new RandomNumberGenerator();
    
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

    public async void PlayParticles(Entity target)
    {
        target.AddChild(this);
        audioEffect.Play();

        for (int i = 0; i < 5; i++)
        {
            AnimatedSprite2D particle = (AnimatedSprite2D) visualEffect.Duplicate();
            AddChild(particle);
            particle.GlobalPosition = target.GetGlobalPosition() + new Vector2(rand.RandiRange(-50, 50), rand.RandiRange(-100, 0));
            Debug.Print(particle.GlobalPosition.ToString());
            particle.Play();
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
        
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        target.RemoveChild(this);
        QueueFree();
    }
}
