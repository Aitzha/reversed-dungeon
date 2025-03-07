using Godot;
using System;

[GlobalClass]
public partial class AttackDebuff : BaseEffect
{
    public override bool IsStatusEffect { get; } = true;

    public AttackDebuff() {}
    
    public AttackDebuff(int duration, int magnitude, Entity target, Entity caster)
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
        
        target.entityData.attackPower -= magnitude;
        target.entityData.effectsInAction.Add(this);
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new AttackDebuff(duration, magnitude, target, caster);
    }
}
