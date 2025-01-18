using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : Node
{
    public string EntityName;
    public int Health;
    public int MaxHealth;
    public int Guard;
    public int AttackPower;
    public bool IsParalyzed;
    public List<BaseEffect> AppliedEffects;

    public void ProcessAppliedEffects()
    {
    }

    public void ApplyEffect(BaseEffect effect, Entity caster)
    {
        
    }
}
