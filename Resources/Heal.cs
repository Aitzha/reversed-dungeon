using Godot;
using System;

[GlobalClass]
public partial class Heal : BaseEffect
{
    public override bool IsStatusEffect { get; } = false;
    public override EffectType type { get; } = EffectType.Heal;

    public Heal() {}
    
    public Heal(int magnitude, Entity target, Entity caster)
    {
        duration = 0;
        this.magnitude = magnitude;
        this.target = target;
        this.caster = caster;
        PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/BattleFX.tscn");
        BattleFX fxInstance = (BattleFX)packedScene.Instantiate();
        fxInstance.Setup(FXType.Heal);
        fx = fxInstance;
    }

    public override void ApplyEffect()
    {
        target.entityData.health = Math.Min(target.entityData.health + magnitude, target.entityData.maxHealth);
        fx.Play(target, magnitude);
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Heal(magnitude, target, caster);
    }
}
