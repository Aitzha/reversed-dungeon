using Godot;
using System;

[GlobalClass]
public partial class Bleed : BaseEffect
{
    public override bool IsStatusEffect { get; } = true;

    public Bleed() {}
    
    public Bleed(int duration, int magnitude, Entity target, Entity caster)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        this.target = target;
        this.caster = caster;
        PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/BattleFX.tscn");
        BattleFX fxInstance = (BattleFX)packedScene.Instantiate();
        fxInstance.Setup(FXType.Slash);
        fx = fxInstance;
        type = EffectType.Bleed;
    }

    public override void ApplyEffect()
    {
        if (!target.isActive)
            return;
        
        duration--;
        target.entityData.health = Mathf.Max(target.entityData.health - magnitude, 0);
        fx.Play(target, magnitude);
        target.Shake();
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Bleed(duration, magnitude, target, caster);
    }
}
