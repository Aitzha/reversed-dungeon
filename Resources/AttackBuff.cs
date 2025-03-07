using Godot;
using System;

[GlobalClass]
public partial class AttackBuff : BaseEffect
{
    public override bool IsStatusEffect { get; } = true;
}
