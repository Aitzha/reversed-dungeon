using Godot;
using System.Collections.Generic;

public partial class Entity : Node2D
{
    [Export] private Sprite2D glowSprite;
    [Export] private EntityUI entityUI;
    
    public EntityData entityData;

    public override void _Ready()
    {
        entityUI.UpdateUI(entityData);
    }

    public void ProcessAppliedEffects()
    {
        List<BaseEffect> expiredEffects = new List<BaseEffect>();

        foreach (BaseEffect effect in entityData.appliedEffects)
        {
            if (effect.IsExpired)
                expiredEffects.Add(effect);
        }

        foreach (BaseEffect effect in expiredEffects)
            entityData.appliedEffects.Remove(effect);
        
        entityUI.UpdateUI(entityData);
    }

    public void ApplyEffect(BaseEffect effect, Entity caster)
    {
        if (effect is InstantEffect instantEffect)
        {
            instantEffect.Activate(caster, this);
        } 
        
        if (effect is ContinuousEffect continuousEffect)
        {
            
        } 
        
        if (effect is BuffDebuffEffect buffDebuffEffect)
        {
            
        }
        
        entityUI.UpdateUI(entityData);
    }

    public void ToggleGlow()
    {
        glowSprite.Visible = !glowSprite.Visible;
    }
}

public class EntityData
{
    public string entityName { get; set; }
    public int health { get; set; }
    public int maxHealth { get; set; }
    public int guard { get; set; }
    public int attackPower { get; set; }
    public bool isParalyzed { get; set; }
    public List<BaseEffect> appliedEffects { get; set; }
    
    public EntityData(string entityName, int maxHealth)
    {
        this.entityName = entityName;
        this.maxHealth = maxHealth;
        health = maxHealth;
        guard = 0;
        attackPower = 0;
        isParalyzed = false;
        appliedEffects = new List<BaseEffect>();
    }

    public EntityData() {}
}


