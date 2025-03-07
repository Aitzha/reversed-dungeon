using Godot;
using System;

[GlobalClass]
public partial class Guard : BaseEffect
{
    public override bool IsStatusEffect { get; } = false;
}
