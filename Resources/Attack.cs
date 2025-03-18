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
        PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/BattleFX.tscn");
        BattleFX fxInstance = (BattleFX)packedScene.Instantiate();
        fxInstance.Setup(FXType.Slash);
        fx = fxInstance;
    }

    public override void ApplyEffect()
    {
        int casterAttack = Math.Max(0, magnitude + caster.attackPower);
        int damageOnGuard = Math.Min(target.guard, casterAttack);
        target.guard -= damageOnGuard;
        target.entityData.health = Mathf.Max(target.entityData.health - (casterAttack - damageOnGuard), 0);
        fx.Play(target, casterAttack);
        target.Shake();
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Attack(magnitude, target, caster);
    }
}
