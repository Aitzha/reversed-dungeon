using Godot;
using System;

[GlobalClass]
public partial class RegularEffect : BaseEffect
{
    [Export] public RegularEffectType type = RegularEffectType.Attack;

    public RegularEffect() {}
    
    public RegularEffect(int magnitude, RegularEffectType type)
    {
        this.magnitude = magnitude;
        this.type = type;
    }
    
    public RegularEffect(int magnitude, Entity target, Entity caster, RegularEffectType type)
    {
        this.magnitude = magnitude;
        this.target = target;
        this.caster = caster;
        this.type = type;
    }

    public override void ApplyEffect()
    {
        switch (type)
        {
            case RegularEffectType.Attack:
                int casterAttack = Math.Max(0, magnitude + caster.entityData.attackPower);
                int damageOnGuard = Math.Min(target.entityData.guard, casterAttack);
                target.entityData.guard -= damageOnGuard;
                target.entityData.health -= (casterAttack - damageOnGuard);
                break;
            case RegularEffectType.Guard:
                target.entityData.guard += magnitude;
                break;
            case RegularEffectType.Heal:
                target.entityData.health = Math.Min(target.entityData.health + magnitude, target.entityData.maxHealth);
                break;
        }
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new RegularEffect(magnitude, target, caster, type);
    }
}

public enum RegularEffectType
{
    Attack,
    Guard,
    Heal
}
