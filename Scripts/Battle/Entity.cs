using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Entity : Node2D
{
    private static Dictionary<StatusEffectType, TriggerType> triggerType = new()
    {
        {StatusEffectType.Poison, TriggerType.NextTurn},
        {StatusEffectType.Bleed, TriggerType.NextTurn},
        {StatusEffectType.AttackBuff, TriggerType.Instant},
        {StatusEffectType.AttackDebuff, TriggerType.NextTurn},
        {StatusEffectType.Paralyze, TriggerType.NextTurn},
        {StatusEffectType.Regeneration, TriggerType.NextTurn}
    };
    
    private static Dictionary<StatusEffectType, EffectDuration> effectDuration = new()
    {
        {StatusEffectType.Poison, EffectDuration.OnApply},
        {StatusEffectType.Bleed, EffectDuration.OnApply},
        {StatusEffectType.AttackBuff, EffectDuration.OnTurnEnd},
        {StatusEffectType.AttackDebuff, EffectDuration.OnTurnEnd},
        {StatusEffectType.Paralyze, EffectDuration.OnTurnEnd},
        {StatusEffectType.Regeneration, EffectDuration.OnApply}
    };
    
    [Export] private Sprite2D glowSprite;
    [Export] private EntityUI entityUI;
    
    public EntityData entityData;
    public bool isPlayerAlly = false;

    public override void _Ready()
    {
        entityUI.UpdateUI(entityData);
    }

    public void StartTurn()
    {
        entityData.guard = 0;
        entityData.attackPower = 0;
        entityData.isParalyzed = false;
        
        List<StatusEffect> expiredEffects = new List<StatusEffect>();
        
        foreach (StatusEffect effect in entityData.appliedEffects)
        {
            effect.ApplyEffect();

            if (effectDuration[effect.type] == EffectDuration.OnApply)
                effect.turnsLeft--;
            
            if (effect.turnsLeft <= 0)
                expiredEffects.Add(effect);
        }

        foreach (StatusEffect effect in expiredEffects)
            entityData.appliedEffects.Remove(effect);
        
        entityUI.UpdateUI(entityData);
        if (entityData.health <= 0)
            BattleManager.instance.DestroyEntity(this);
    }

    public void FinishTurn()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();
        
        foreach (StatusEffect effect in entityData.appliedEffects)
        {
            if (effectDuration[effect.type] == EffectDuration.OnTurnEnd)
                effect.turnsLeft--;
            
            if (effect.turnsLeft <= 0)
                expiredEffects.Add(effect);
        }
        
        foreach (StatusEffect effect in expiredEffects)
            entityData.appliedEffects.Remove(effect);
        
        entityUI.UpdateUI(entityData);
    }

    public void ApplyEffect(BaseEffect effect, Entity caster)
    {
        BaseEffect effectCopy = effect.Clone(this, caster);
        
        if (effectCopy is RegularEffect || 
            (effectCopy is StatusEffect statusEffect && triggerType[statusEffect.type] == TriggerType.Instant))
            effectCopy.ApplyEffect();

        if (effectCopy is StatusEffect newStatusEffect)
        {
            // Increase duration of already existing effect with similar parameters or add new effect if none is found
            StatusEffect oldStatusEffect = entityData.appliedEffects.FirstOrDefault(appliedEffect =>
                appliedEffect.magnitude == newStatusEffect.magnitude && appliedEffect.type == newStatusEffect.type);

            if (oldStatusEffect == null)
                entityData.appliedEffects.Add(newStatusEffect);
            else 
                oldStatusEffect.turnsLeft += newStatusEffect.turnsLeft;
        } 
    }
    
    public void ApplyEffects(Card card, Entity caster)
    {
        foreach (BaseEffect effect in card.cardData.effects)
        {
            ApplyEffect(effect, caster);
        }
        
        entityUI.UpdateUI(entityData);
        if (entityData.health <= 0)
            BattleManager.instance.DestroyEntity(this);
    }

    public void ToggleGlow()
    {
        glowSprite.Visible = !glowSprite.Visible;
    }

    public void damageFX()
    {
        PackedScene slashPackedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/SlashFX.tscn");
        BattleFX slashInstance = (BattleFX)slashPackedScene.Instantiate();
        Node node = GetParent().GetNode<Node>("FXStorage");
        slashInstance.Play(node, GlobalPosition);
        Shake();
    }

    public void healFX()
    {
        PackedScene healPackedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/HealFX.tscn");
        BattleFX healInstance = (BattleFX)healPackedScene.Instantiate();
        Node node = GetParent().GetNode<Node>("FXStorage");
        healInstance.PlayParticles(node, GlobalPosition);
    }

    private async void Shake()
    {
        // length of effect in seconds
        double length = 0.2;
        double period = length / 6;

        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == 0)
                Position += new Vector2(5, 0);
            if (i % 2 == 1)
                Position += new Vector2(-5, 0);
            
            await ToSignal(GetTree().CreateTimer(period), "timeout");
        }
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
    public List<StatusEffect> appliedEffects { get; set; }
    
    
    public EntityData(string entityName, int maxHealth)
    {
        this.entityName = entityName;
        this.maxHealth = maxHealth;
        health = maxHealth;
        guard = 0;
        attackPower = 0;
        isParalyzed = false;
        appliedEffects = new List<StatusEffect>();
    }

    public EntityData() {}
}

public enum TriggerType
{
    Instant,
    NextTurn
}

public enum EffectDuration
{
    OnApply,
    OnTurnEnd
}


