using Godot;
using System.Collections.Generic;
using System.Diagnostics;

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

    public void Setup(List<CardData> playerCards, List<EntityData> playerTeamData, List<EntityData> enemyTeamData)
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
        LoadEntities();
        battleInterface.FillHand();
    }
    
    public void EndPlayerTurn()
    {
        player.FinishTurn();
        
        playerTurn = false;
        EnemyTeamPerformAction();
        
        playerTurn = true;
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

    private void LoadEntities()
    {
        foreach (Entity player in playerTeam)
            AddChild(player);
        
        foreach (Entity enemy in enemyTeam)
            AddChild(enemy);
    }

    private void EnemyTeamPerformAction()
    {
        Effect effect = new Effect(EffectType.Attack, EffectSubtype.None, FirstTriggerTiming.Immediate, DurationReductionTiming.OnEffectApply, 1, 1);
        foreach (Entity enemy in enemyTeam)
        {
            enemy.StartTurn();
            if (player.entityData.health <= 0)
                return;
            
            player.ApplyEffect(effect, enemy);
        }
        
        foreach (Entity enemy in enemyTeam)
            enemy.FinishTurn();
    }
}
