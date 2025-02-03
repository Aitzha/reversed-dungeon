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

    public async void Play(Node parent, Vector2 globalPosition)
    {
        parent.AddChild(this);
        GlobalPosition = globalPosition;
        visualEffect.Play();
        audioEffect.Play();
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        parent.RemoveChild(this);
        QueueFree();
    }

    public async void PlayParticles(Node parent, Vector2 globalPosition)
    {
        parent.AddChild(this);
        audioEffect.Play();

        for (int i = 0; i < 5; i++)
        {
            AnimatedSprite2D particle = (AnimatedSprite2D) visualEffect.Duplicate();
            AddChild(particle);
            particle.GlobalPosition = globalPosition + new Vector2(rand.RandiRange(-50, 50), rand.RandiRange(-100, 0));
            particle.Play();
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
        
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        parent.RemoveChild(this);
        QueueFree();
    }
}
