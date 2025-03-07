using Godot;
using System;

[GlobalClass]
public partial class Attack : BaseEffect
{
    public override bool IsStatusEffect { get; } = false;
}
