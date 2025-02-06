using Godot;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Entity : Node2D
{
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
        
        List<Effect> expiredEffects = new List<Effect>();
        
        foreach (Effect effect in entityData.appliedEffects)
        {
            effect.ApplyEffect();
            
            if (effect.duration <= 0)
                expiredEffects.Add(effect);
        }

        foreach (Effect effect in expiredEffects)
            entityData.appliedEffects.Remove(effect);
        
        entityUI.UpdateUI(entityData);
        if (entityData.health <= 0)
            BattleManager.instance.DestroyEntity(this);
    }

    public void FinishTurn()
    {
        List<Effect> expiredEffects = new List<Effect>();
        
        foreach (Effect effect in entityData.appliedEffects)
        {
            if (effect.durationReductionTiming == DurationReductionTiming.OnTurnEnd)
                effect.duration--;
            
            if (effect.duration <= 0)
                expiredEffects.Add(effect);
        }
        
        foreach (Effect effect in expiredEffects)
            entityData.appliedEffects.Remove(effect);
    }

    public void ApplyEffect(Effect effect, Entity caster)
    {
        Effect effectCopy = effect.Clone(this, caster);
        
        if (effectCopy.firstTriggerTiming == FirstTriggerTiming.Immediate)
            effectCopy.ApplyEffect();
        
        if (effectCopy.duration > 0)
            entityData.appliedEffects.Add(effectCopy);
    }
    
    public void ApplyEffects(Card card, Entity caster)
    {
        int previousHealth = entityData.health;
        
        foreach (Effect effect in card.cardData.Effects)
        {
            ApplyEffect(effect, caster);
        }
        
        if (previousHealth > entityData.health)
        {
            PackedScene slashPackedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/SlashFX.tscn");
            BattleFX slashInstance = (BattleFX)slashPackedScene.Instantiate();
            Node node = GetParent().GetNode<Node>("FXStorage");
            slashInstance.Play(node, GlobalPosition);
            Shake();
        }
        else if (previousHealth < entityData.health)
        {
            PackedScene healPackedScene = (PackedScene)ResourceLoader.Load("res://Scenes/Battle/FX/HealFX.tscn");
            BattleFX healInstance = (BattleFX)healPackedScene.Instantiate();
            Node node = GetParent().GetNode<Node>("FXStorage");
            healInstance.PlayParticles(node, GlobalPosition);
        }
        
        entityUI.UpdateUI(entityData);
        if (entityData.health <= 0)
            BattleManager.instance.DestroyEntity(this);
    }

    public void ToggleGlow()
    {
        glowSprite.Visible = !glowSprite.Visible;
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
    public List<Effect> appliedEffects { get; set; }
    
    public EntityData(string entityName, int maxHealth)
    {
        this.entityName = entityName;
        this.maxHealth = maxHealth;
        health = maxHealth;
        guard = 0;
        attackPower = 0;
        isParalyzed = false;
        appliedEffects = new List<Effect>();
    }

    public EntityData() {}
}


