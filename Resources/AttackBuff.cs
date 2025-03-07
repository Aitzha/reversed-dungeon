using Godot;
using System;

[GlobalClass]
public partial class AttackBuff : BaseEffect
{
    public override bool IsStatusEffect { get; } = true;

    public AttackBuff() {}
    
    public AttackBuff(int duration, int magnitude, Entity target, Entity caster)
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
        
        target.entityData.attackPower += magnitude;
        target.entityData.effectsInAction.Add(this);
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new AttackBuff(duration, magnitude, target, caster);
    }
}
