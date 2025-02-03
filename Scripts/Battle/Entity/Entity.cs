using Godot;
using System.Collections.Generic;

public partial class Entity : Node2D
{
    [Export] private Sprite2D glowSprite;
    [Export] private EntityUI entityUI;
    
    public EntityData entityData;
    public bool isAlly = false;

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
        BattleManager.instance.DestroyEntity(this);
    }

    public void ApplyEffect(BaseEffect effect, Entity caster)
    {
        int previousHealth = entityData.health;
        
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
        
        if (previousHealth > entityData.health)
        {
            PackedScene slashPackedScene = (PackedScene)ResourceLoader.Load("res://Scenes/BattleScenes/FX/SlashFX.tscn");
            DigitalEffect slashInstance = (DigitalEffect)slashPackedScene.Instantiate();
            Node node = GetParent().GetNode<Node>("FXStorage");
            slashInstance.Play(node, GlobalPosition);
            Shake();
        }
        else
        {
            // this is temp
            PackedScene healPackedScene = (PackedScene)ResourceLoader.Load("res://Scenes/BattleScenes/FX/HealFX.tscn");
            DigitalEffect healInstance = (DigitalEffect)healPackedScene.Instantiate();
            Node node = GetParent().GetNode<Node>("FXStorage");
            healInstance.PlayParticles(node, GlobalPosition);
        }
        
        entityUI.UpdateUI(entityData);
        if (entityData.health <= 0)
            BattleManager.instance.DestroyEntity(this);
    }
    
    public void ApplyEffects(Card card, Entity caster)
    {
        foreach (BaseEffect effect in card.cardData.Effects)
        {
            ApplyEffect(effect, caster);
        }
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


