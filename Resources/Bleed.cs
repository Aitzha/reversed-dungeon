using Godot;
using System;

[GlobalClass]
public partial class Bleed : BaseEffect
{
    public override bool IsStatusEffect { get; } = true;
}
