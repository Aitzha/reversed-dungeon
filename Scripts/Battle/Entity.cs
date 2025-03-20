using Godot;
using System.Collections.Generic;
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
    
    public int guard { get; set; }
    public int attackPower { get; set; }
    public bool isParalyzed { get; set; }
    public List<BaseEffect> statusEffects { get; set; } = new();
    public List<BaseEffect> effectsInAction { get; set; } = new();

    public BaseEffect nextAction;

    public override void _Ready()
    {
        entityUI.UpdateUI(this);
    }

    public async Task StartTurn()
    {
        isActive = true;
        guard = 0;
        attackPower = 0;
        isParalyzed = false;
        
        List<BaseEffect> expiredEffects = new List<BaseEffect>();
        
        foreach (BaseEffect effect in statusEffects)
        {
            effect.ApplyEffect();
            entityUI.UpdateUI(this);
            await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
            
            if (effect.duration <= 0)
                expiredEffects.Add(effect);
        }

        foreach (BaseEffect effect in expiredEffects)
            statusEffects.Remove(effect);
        
        entityUI.UpdateUI(this);
        if (entityData.health <= 0)
            BattleManager.instance.KillEntity(this);
    }

    public void ChooseAction()
    {
        List<Entity> targets;
    
        if (isPlayerAlly)
            targets = BattleManager.instance.enemyTeam;
        else
            targets = BattleManager.instance.playerTeam;
        
        // Currently only focus on player and entity itself
        Entity player = BattleManager.instance.player;
        Attack attackAction = entityData.possibleActions.Where(effect => effect is Attack).Cast<Attack>().ToList()[0];
        Guard guardAction = entityData.possibleActions.Where(effect => effect is Guard).Cast<Guard>().ToList()[0];
        Heal healAction = entityData.possibleActions.Where(effect => effect is Heal).Cast<Heal>().ToList()[0];
        
        float attackScore = attackPower * 2f + attackAction.magnitude * 3f - player.guard;
        float guardScore = player.attackPower * 2f + (float)entityData.health / entityData.maxHealth * guardAction.magnitude * 0.3f + attackPower * -2;
        float healScore = healAction.magnitude / ((float)entityData.health / entityData.maxHealth * healAction.magnitude) - 1 + player.attackPower * 2f;

        if (attackScore >= guardScore && attackScore >= healScore)
            nextAction = attackAction;
        else if (guardScore > healScore)
            nextAction = guardAction;
        else
            nextAction = healAction;
        
        entityUI.DisplayAction(nextAction);
    }
    
    public async Task PerformAction(List<Entity> targets) // only for AI
    {
        entityUI.HideAction();

        if (nextAction is Attack)
        {
            Tween tween = GetTree().CreateTween();
            Vector2 startPos = Position;
            Vector2 targetPos = Position + new Vector2(-50, 0);
            tween.TweenProperty(this, "position", targetPos, 0.3f);
            tween.TweenProperty(this, "position", startPos, 0.3f);
                
            await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
            targets[0].ApplyEffect(nextAction, this);
        }
        else
        {
            ApplyEffect(nextAction, this);
        }
        
        await ToSignal(GetTree().CreateTimer(0.75f), "timeout");
    }

    // public async Task PerformAction(List<Entity> targets) // only for AI
    // {
    //     entityUI.HideAction();
    //     
    //     Card card = (Card) GD.Load<PackedScene>("res://Scenes/Battle/UI/Card.tscn").Instantiate();
    //     CardData cardData = ResourceLoader.Load<CardData>("res://Data/Cards/EnemyCards/claw_attack.tres");
    //     card.cardData = cardData;
    //     card.playerCard = false;
    //     
    //     Tween tweenPos;
    //     Tween tweenScaleColor;
    //     tweenPos = GetTree().CreateTween();
    //     tweenScaleColor = GetTree().CreateTween();
    //         
    //     tweenPos.SetTrans(Tween.TransitionType.Quad);
    //     tweenPos.SetEase(Tween.EaseType.InOut);
    //         
    //     tweenScaleColor.SetTrans(Tween.TransitionType.Linear);
    //     tweenScaleColor.SetParallel();
    //         
    //     card.Scale = Vector2.Zero;
    //     card.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    //     card.Position = Position + new Vector2(0, -64);
    //         
    //     Vector2 targetPos = targets[0].Position + new Vector2(-GameSettings.cardWidth / 2, -128);
    //     Vector2 finalScale = new Vector2(1, 1);
    //         
    //     BattleManager.instance.AddChild(card);
    //     tweenScaleColor.TweenProperty(card, "scale", finalScale, 1.0f);
    //     tweenScaleColor.TweenProperty(card, "modulate", new Color(1f, 1f, 1f, 0.7f), 1.0f);
    //     tweenPos.TweenProperty(card, "position", targetPos, 1.0f);
    //         
    //     await ToSignal(GetTree().CreateTimer(1.1f), "timeout");
    //     BattleManager.instance.RemoveChild(card);
    //         
    //     targets[0].ApplyEffects(card, this);
    // }

    public void FinishTurn()
    {
        List<BaseEffect> expiredEffects = new List<BaseEffect>();

        foreach (BaseEffect effect in effectsInAction)
        {
            effect.duration--;
            
            if (effect.duration == 0)
                expiredEffects.Add(effect);
        }
        
        effectsInAction.Clear();

        foreach (BaseEffect effect in expiredEffects)
            statusEffects.Remove(effect);
        
        entityUI.UpdateUI(this);
        isActive = false;
    }

    public void ApplyEffect(BaseEffect effect, Entity caster)
    {
        BaseEffect effectCopy = effect.Clone(this, caster);
        effectCopy.ApplyEffect();
        
        if (effectCopy.IsStatusEffect)
        {
            // Increase duration of already existing effect with similar parameters or add new effect if none is found
            BaseEffect oldStatusEffect = statusEffects.FirstOrDefault(statusEffect =>
                statusEffect.magnitude == effectCopy.magnitude && statusEffect.GetType() == effectCopy.GetType());
        
            if (oldStatusEffect == null)
                statusEffects.Add(effectCopy);
            else 
                oldStatusEffect.duration += effect.duration;
        } 
        
        entityUI.UpdateUI(this);
    }
    
    public void ApplyEffects(Card card, Entity caster)
    {
        foreach (BaseEffect effect in card.cardData.effects)
        {
            ApplyEffect(effect, caster);
        }
        
        if (entityData.health <= 0)
            BattleManager.instance.KillEntity(this);
    }

    public void ToggleGlow()
    {
        glowSprite.Visible = !glowSprite.Visible;
    }

    public async void Shake()
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