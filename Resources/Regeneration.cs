using Godot;
using System;

[GlobalClass]
public partial class Regeneration : BaseEffect
{
    public override bool IsStatusEffect { get; } = true;

    public Regeneration() {}
    
    public Regeneration(int duration, int magnitude, Entity target, Entity caster)
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
        target.entityData.health = Math.Min(target.entityData.maxHealth, target.entityData.health + magnitude);
        target.healFX();
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Regeneration(duration, magnitude, target, caster);
    }
}
