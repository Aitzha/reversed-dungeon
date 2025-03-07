using Godot;
using System;

[GlobalClass]
public partial class StatusEffect : BaseEffect
{
    [Export] public StatusEffectType type = StatusEffectType.Poison;

    public StatusEffect() {}

    public StatusEffect(int duration, int magnitude, StatusEffectType type)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        this.type = type;
    }
    
    public StatusEffect(int duration, int magnitude, Entity target, Entity caster, StatusEffectType type)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        this.target = target;
        this.caster = caster;
        this.type = type;
    }
    
    public override void ApplyEffect()
    {
        switch (type)
        {
            case StatusEffectType.Bleed:
                target.entityData.health -= magnitude;
                target.damageFX();
                break;
            case StatusEffectType.Poison:
                target.entityData.health -= magnitude;
                target.damageFX();
                break;
            case StatusEffectType.Regeneration:
                target.entityData.health = Math.Min(target.entityData.maxHealth, target.entityData.health + magnitude);
                target.healFX();
                break;
            case StatusEffectType.AttackBuff:
                target.entityData.attackPower += magnitude;
                break;
            case StatusEffectType.AttackDebuff:
                target.entityData.attackPower -= magnitude;
                break;
            case StatusEffectType.Paralyze:
                target.entityData.isParalyzed = true;
                break;
        }
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new StatusEffect(duration, magnitude, target, caster, type);
    }
}

public enum StatusEffectType
{
    Bleed,
    Poison,
    AttackBuff,
    AttackDebuff,
    Paralyze,
    Regeneration
}