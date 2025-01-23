using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : Node2D
{
    public string EntityName;
    public int Health;
    public int MaxHealth;
    public int Guard;
    public int AttackPower;
    public bool IsParalyzed;
    public List<BaseEffect> AppliedEffects;

    public Entity(string entityName, int maxHealth)
    {
        EntityName = entityName;
        MaxHealth = maxHealth;
        Health = MaxHealth;
        Guard = 0;
        AttackPower = 0;
        IsParalyzed = false;
        AppliedEffects = new List<BaseEffect>();
    }

    public Entity() {}
    
    public void CopyOriginal(Entity original)
    {
        EntityName = original.EntityName;
        Health = original.Health;
        MaxHealth = original.MaxHealth;
        Guard = 0;
        AttackPower = 0;
        IsParalyzed = false;
        AppliedEffects = new List<BaseEffect>();
    }

    public void ProcessAppliedEffects()
    {
    }

    public void ApplyEffect(BaseEffect effect, Entity caster)
    {
        
    }
}
