using Godot;
using System;

[GlobalClass]
public partial class AttackDebuff : BaseEffect
{
    public override bool IsStatusEffect { get; } = true;
}
