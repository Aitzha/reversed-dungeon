using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public partial class Entity : Node2D
{
    [Export] private Sprite2D glowSprite;
    [Export] private EntityUI entityUI;
    
    public EntityData entityData;
    public bool isPlayerAlly = false;
    public bool isActive = false;
    public bool isDead = false;

    public override void _Ready()
    {
        entityUI.UpdateUI(entityData);
    }

    public async Task StartTurn()
    {
        isActive = true;
        entityData.guard = 0;
        entityData.attackPower = 0;
        entityData.isParalyzed = false;
        
        List<BaseEffect> expiredEffects = new List<BaseEffect>();
        
        foreach (BaseEffect effect in entityData.statusEffects)
        {
            effect.ApplyEffect();
            
            if (effect.duration <= 0)
                expiredEffects.Add(effect);
        }

        foreach (BaseEffect effect in expiredEffects)
            entityData.statusEffects.Remove(effect);
        
        entityUI.UpdateUI(entityData);
        if (entityData.health <= 0)
            BattleManager.instance.KillEntity(this);
    }

    public async Task PerformAction(List<Entity> targets) // only for AI (both ally and enemy)
    {
        Card card = (Card) GD.Load<PackedScene>("res://Scenes/Battle/UI/Card.tscn").Instantiate();
        CardData cardData = ResourceLoader.Load<CardData>("res://Data/Cards/EnemyCards/claw_attack.tres");
        card.cardData = cardData;
        card.playerCard = false;
        
        Tween tweenPos;
        Tween tweenScale;
        tweenPos = GetTree().CreateTween();
        tweenScale = GetTree().CreateTween();
            
        tweenPos.SetTrans(Tween.TransitionType.Quad);
        tweenPos.SetEase(Tween.EaseType.InOut);
            
        tweenScale.SetTrans(Tween.TransitionType.Linear);
        tweenScale.SetParallel();
            
        card.Scale = Vector2.Zero;
        card.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        card.Position = Position + new Vector2(0, -64);
            
        Vector2 targetPos = targets[0].Position + new Vector2(-GameSettings.cardWidth / 2, -128);
        Vector2 finalScale = new Vector2(1, 1);
            
        BattleManager.instance.AddChild(card);
        tweenScale.TweenProperty(card, "scale", finalScale, 1.0f);
        tweenScale.TweenProperty(card, "modulate", new Color(1f, 1f, 1f, 0.7f), 1.0f);
        tweenPos.TweenProperty(card, "position", targetPos, 1.0f);
            
        await ToSignal(GetTree().CreateTimer(1.1f), "timeout");
        BattleManager.instance.RemoveChild(card);
            
        targets[0].ApplyEffects(card, this);
    }

    public void FinishTurn()
    {
        List<BaseEffect> expiredEffects = new List<BaseEffect>();

        foreach (BaseEffect effect in entityData.effectsInAction)
        {
            effect.duration--;
            
            if (effect.duration == 0)
                expiredEffects.Add(effect);
        }
        
        entityData.effectsInAction.Clear();

        foreach (BaseEffect effect in expiredEffects)
            entityData.statusEffects.Remove(effect);
        
        entityUI.UpdateUI(entityData);
        isActive = false;
    }

    public void ApplyEffect(BaseEffect effect, Entity caster)
    {
        BaseEffect effectCopy = effect.Clone(this, caster);
        effectCopy.ApplyEffect();
        
        if (effectCopy.IsStatusEffect)
        {
            // Increase duration of already existing effect with similar parameters or add new effect if none is found
            BaseEffect oldStatusEffect = entityData.statusEffects.FirstOrDefault(statusEffect =>
                statusEffect.magnitude == effectCopy.magnitude && statusEffect.GetType() == effectCopy.GetType());
        
            if (oldStatusEffect == null)
                entityData.statusEffects.Add(effectCopy);
            else 
                oldStatusEffect.duration += effect.duration;
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
            BattleManager.instance.KillEntity(this);
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
    public List<BaseEffect> statusEffects { get; set; }
    public List<BaseEffect> effectsInAction { get; set; }
    
    public EntityData(string entityName, int maxHealth)
    {
        this.entityName = entityName;
        this.maxHealth = maxHealth;
        health = maxHealth;
        guard = 0;
        attackPower = 0;
        isParalyzed = false;
        statusEffects = new List<BaseEffect>();
        effectsInAction = new List<BaseEffect>();
    }

    public EntityData() {}
}

