using Godot;
using System;

[GlobalClass]
public partial class Heal : BaseEffect
{
    public override bool IsStatusEffect { get; } = false;

    public Heal() {}
    
    public Heal(int magnitude, Entity target, Entity caster)
    {
        duration = 0;
        this.magnitude = magnitude;
        this.target = target;
        this.caster = caster;
    }

    public override void ApplyEffect()
    {
        target.entityData.health = Math.Min(target.entityData.health + magnitude, target.entityData.maxHealth);
        target.healFX();
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Heal(magnitude, target, caster);
    }
}
