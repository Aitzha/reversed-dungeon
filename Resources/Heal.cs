using Godot;
using System;

[GlobalClass]
public partial class Heal : BaseEffect
{
    public override bool IsStatusEffect { get; } = false;
}
