using Godot;
using System;

[GlobalClass]
public partial class Attack : BaseEffect
{
    public override bool IsStatusEffect { get; } = false;
    
    public Attack() {}

    public Attack(int magnitude, Entity target, Entity caster)
    {
        duration = 0;
        this.magnitude = magnitude;
        this.target = target;
        this.caster = caster;
    }

    public override void ApplyEffect()
    {
        int casterAttack = Math.Max(0, magnitude + caster.entityData.attackPower);
        int damageOnGuard = Math.Min(target.entityData.guard, casterAttack);
        target.entityData.guard -= damageOnGuard;
        target.entityData.health = Mathf.Max(target.entityData.health - (casterAttack - damageOnGuard), 0);
        target.damageFX();
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Attack(magnitude, target, caster);
    }
}
