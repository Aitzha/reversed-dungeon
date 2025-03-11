using Godot;
using System;

[GlobalClass]
public partial class Guard : BaseEffect
{
    public override bool IsStatusEffect { get; } = false;

    public Guard() {}
    
    public Guard(int magnitude, Entity target, Entity caster)
    {
        duration = 0;
        this.magnitude = magnitude;
        this.target = target;
        this.caster = caster;
        PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/BattleFX.tscn");
        BattleFX fxInstance = (BattleFX)packedScene.Instantiate();
        fxInstance.Setup(FXType.None);
        fx = fxInstance;
    }

    public override void ApplyEffect()
    {
        target.entityData.guard += magnitude;
    }

    public override BaseEffect Clone(Entity target, Entity caster)
    {
        return new Guard(magnitude, target, caster);
    }
}
