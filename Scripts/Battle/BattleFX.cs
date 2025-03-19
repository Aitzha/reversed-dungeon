using System.Diagnostics;
using Godot;

public partial class BattleFX : Node2D
{
    private AudioStream healSound = GD.Load<AudioStream>("res://Sound/heal.wav");
    private AudioStream slashSound = GD.Load<AudioStream>("res://Sound/slash.wav");
    
    [Export] private AnimatedSprite2D visualEffect;
    [Export] private AudioStreamPlayer2D audioEffect;
    [Export] private Label numberLabel;
    
    private RandomNumberGenerator rand = new();
    private bool particles = false;
    private NumberLabelType numberLabelType;

    public void Setup(FXType type)
    {
        numberLabel.Visible = false;
        
        switch (type)
        {
            case FXType.None:
                visualEffect.Animation = "default";
                particles = false;
                numberLabelType = NumberLabelType.None;
                audioEffect.Stream = null;
                break;
            case FXType.Heal:
                visualEffect.Animation = "Heal";
                particles = true;
                numberLabelType = NumberLabelType.Heal;
                audioEffect.Stream = healSound;
                break;
            case FXType.Slash:
                visualEffect.Animation = "Slash";
                particles = false;
                numberLabelType = NumberLabelType.Damage;
                audioEffect.Stream = slashSound;
                break;
        }
    }
    
    public override void _Ready() {}

    public async void Play(Entity entity, int amount = 0)
    {
        BattleManager.instance.AddChild(this);
        GlobalPosition = entity.GlobalPosition;
        
        if (audioEffect.Stream != null)
            audioEffect.Play();

        if (numberLabelType != NumberLabelType.None)
        {
            Tween tweenPos;
            Tween tweenColor;
            tweenPos = GetTree().CreateTween();
            tweenColor = GetTree().CreateTween();
            
            tweenPos.SetTrans(Tween.TransitionType.Linear);
            tweenColor.SetTrans(Tween.TransitionType.Linear);


            Color color = numberLabelType switch
            {
                NumberLabelType.Heal => Colors.Lime,
                NumberLabelType.Damage => Colors.Red,
                _ => Colors.White
            };
            
            color.A = 0;
            numberLabel.Modulate = color;
            numberLabel.Position = new Vector2((entity.isPlayerAlly ? 20 : -60), 0);
            numberLabel.Text = (numberLabelType == NumberLabelType.Damage ? "-" : "+") + amount;
            numberLabel.Visible = true;
            
            Vector2 targetPos = numberLabel.Position + new Vector2(0, -50);
            
            tweenPos.TweenProperty(numberLabel, "position", targetPos, 2.0f);
            tweenColor.TweenProperty(numberLabel, "modulate", color with { A = 1 }, 1.0f);
            tweenColor.TweenProperty(numberLabel, "modulate", color with { A = 0 }, 1.0f);
        }
        
        if (particles)
        {
            visualEffect.Visible = false;
            for (int i = 0; i < 5; i++)
            {
                AnimatedSprite2D particle = (AnimatedSprite2D) visualEffect.Duplicate();
                particle.Visible = true;
                AddChild(particle);
                particle.GlobalPosition = GlobalPosition + new Vector2(rand.RandiRange(-50, 50), rand.RandiRange(-100, 0));
                particle.Play();
                await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            }
        }
        else
        {
            visualEffect.Play();
        }
        
        
        await ToSignal(GetTree().CreateTimer(3.0f), "timeout");
        BattleManager.instance.RemoveChild(this);
    }
}

public enum FXType
{
    None,
    Heal,
    Slash
}

public enum NumberLabelType
{
    None, 
    Damage,
    Heal
}
