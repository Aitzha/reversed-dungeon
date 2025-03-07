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
    }

    public override void ApplyEffect()
    {
        if (!target.isActive)
            return;
        
        duration--;
        target.entityData.health -= magnitude;
        target.damageFX();
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Bleed(duration, magnitude, target, caster);
    }
}
