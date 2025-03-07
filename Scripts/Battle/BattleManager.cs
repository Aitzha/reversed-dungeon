using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using Godot.Collections;

public partial class BattleManager : Node
{
    [Export] private BattleInterface battleInterface;
    [Export] private PackedScene endScreen;

    [Signal] public delegate void BattleWonEventHandler();
    
    public static BattleManager instance;
    public int playerMana = 2;
    public int playerHandCapacity = 3;
    public int playerManaCapacity = 2;
    public Entity player;

    private bool playerTurn = true;
    private List<Entity> playerTeam = new();
    private List<Entity> enemyTeam = new();
    
    private List<Vector2> playerTeamPositions = new()
    {
        new Vector2(110, 220)
    };

    private List<Vector2> enemyTeamPositions = new()
    {
        new Vector2(465, 205),
        new Vector2(570, 270),
        new Vector2(360, 140),
    };

    public void Setup(Array<CardData> playerCards, List<EntityData> playerTeamData, List<EntityData> enemyTeamData)
    {
        instance = this;
        battleInterface.playerCards = playerCards;
        
        for (int i = 0; i < playerTeamData.Count; i++) 
        {
            PackedScene packedPlayerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Battle/Characters/Player/" + playerTeamData[i].entityName + ".tscn");

            Entity playerTeamMember = (Entity)packedPlayerScene.Instantiate();
            playerTeamMember.entityData = playerTeamData[i];
            
            playerTeamMember.Position = playerTeamPositions[i];
            playerTeamMember.isPlayerAlly = true;
            playerTeam.Add(playerTeamMember);
        }
        
        player = playerTeam[0];
        
        for (int i = 0; i < enemyTeamData.Count; i++) 
        {
            PackedScene packedEnemyScene = ResourceLoader.Load<PackedScene>("res://Scenes/Battle/Characters/Enemies/" + enemyTeamData[i].entityName + ".tscn");

            Entity enemyTeamMember = (Entity)packedEnemyScene.Instantiate();
            enemyTeamMember.entityData = enemyTeamData[i];

            enemyTeamMember.Position = enemyTeamPositions[i];
            enemyTeamMember.isPlayerAlly = false;
            enemyTeam.Add(enemyTeamMember);
        }
    }

    public override void _Ready()
    {
        foreach (Entity playerTeamMember in playerTeam)
            AddChild(playerTeamMember);
        
        foreach (Entity enemyTeamMember in enemyTeam)
            AddChild(enemyTeamMember);
        
        StartPlayerTurn();
    }
    
    public void EndPlayerTurn()
    {
        player.FinishTurn();
        
        playerTurn = false;
        player.isActive = false;
        EnemyTeamPerformAction();
    }

    public void StartPlayerTurn()
    {
        playerTurn = true;
        player.isActive = true;
        player.StartTurn();
        playerMana = playerManaCapacity;
        battleInterface.FillHand();
    }

    public void DestroyEntity(Entity entity)
    {
        if (entity.entityData.health > 0)
            return;
        
        if (entity.isPlayerAlly)
        {
            playerTeam.Remove(entity);
            RemoveChild(entity);
            entity.QueueFree();

            if (entity == player)
            {
                Node endScreenInstance = endScreen.Instantiate();
                endScreenInstance.GetNode<Label>("Text").Text = "You Lose";
                AddChild(endScreenInstance);
            }
        }
        else
        {
            enemyTeam.Remove(entity);
            RemoveChild(entity);
            entity.QueueFree();

            if (enemyTeam.Count == 0)
            {
                EmitSignal(SignalName.BattleWon);
            }
        }
    }

    private async void EnemyTeamPerformAction()
    {
        Card card = (Card) GD.Load<PackedScene>("res://Scenes/Battle/UI/Card.tscn").Instantiate();
        CardData cardData = ResourceLoader.Load<CardData>("res://Data/Cards/EnemyCards/claw_attack.tres");
        card.cardData = cardData;
        card.playerCard = false;
        
        Tween tweenPos;
        Tween tweenScale;
        foreach (Entity enemy in enemyTeam)
        {
            enemy.isActive = true;
            enemy.StartTurn();
            
            await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

            tweenPos = GetTree().CreateTween();
            tweenScale = GetTree().CreateTween();
            
            tweenPos.SetTrans(Tween.TransitionType.Quad);
            tweenPos.SetEase(Tween.EaseType.InOut);
            
            tweenScale.SetTrans(Tween.TransitionType.Linear);
            tweenScale.SetParallel();
            
            card.Scale = Vector2.Zero;
            card.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            card.Position = enemy.Position + new Vector2(0, -64);
            
            Vector2 targetPos = player.Position + new Vector2(-GameSettings.cardWidth / 2, -128);
            Vector2 finalScale = new Vector2(1, 1);
            
            AddChild(card);
            tweenScale.TweenProperty(card, "scale", finalScale, 1.0f);
            tweenScale.TweenProperty(card, "modulate", new Color(1f, 1f, 1f, 0.7f), 1.0f);
            tweenPos.TweenProperty(card, "position", targetPos, 1.0f);
            
            await ToSignal(GetTree().CreateTimer(2f), "timeout");
            RemoveChild(card);
            
            player.ApplyEffects(card, enemy);
            enemy.FinishTurn();
            enemy.isActive = false;
            
            if (player.entityData.health <= 0)
                return;
        }
            
        
        StartPlayerTurn();
    }
}
