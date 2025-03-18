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
        PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/BattleFX.tscn");
        BattleFX fxInstance = (BattleFX)packedScene.Instantiate();
        fxInstance.Setup(FXType.None);
        fx = fxInstance;
        type = EffectType.Debuff;
    }

    public override void ApplyEffect()
    {
        if (!target.isActive)
            return;
        
        target.attackPower -= magnitude;
        target.effectsInAction.Add(this);
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new AttackDebuff(duration, magnitude, target, caster);
    }
}
