using Godot;
using System.Collections.Generic;

public partial class BattleManager : Node
{
    [Export] public BattleInterface battleInterface;

    private bool playerTurn = true;
    private List<Entity> playerTeam = new List<Entity>();
    private List<Entity> enemyTeam = new List<Entity>();
    
    private List<Vector2> playerTeamPositions = new List<Vector2>
    {
        new Vector2(110, 230)
    };

    private List<Vector2> enemyTeamPositions = new List<Vector2>
    {
        new Vector2(450, 235),
        new Vector2(560, 270),
        new Vector2(360, 220),
    };

    public void Setup(List<CardData> playerCards, List<EntityData> playerTeamData, List<EntityData> enemyTeamData)
    {
        battleInterface.playerCards = playerCards;
        
        for (int i = 0; i < playerTeamData.Count; i++) 
        {
            PackedScene packedPlayerScene = ResourceLoader.Load<PackedScene>("res://Scenes/BattleScenes/Characters/Player/" + playerTeamData[i].entityName + ".tscn");

            Entity player = (Entity)packedPlayerScene.Instantiate();
            player.entityData = playerTeamData[i];
            
            player.Position = playerTeamPositions[i];
            playerTeam.Add(player);
        }
        
        for (int i = 0; i < enemyTeamData.Count; i++) 
        {
            PackedScene packedEnemyScene = ResourceLoader.Load<PackedScene>("res://Scenes/BattleScenes/Characters/Enemies/" + enemyTeamData[i].entityName + ".tscn");

            Entity enemy = (Entity)packedEnemyScene.Instantiate();
            enemy.entityData = enemyTeamData[i];

            enemy.Position = enemyTeamPositions[i];
            enemyTeam.Add(enemy);
        }
    }

    public override void _Ready()
    {
        LoadEntities();
        BattleEventBus.instance.EndTurn += EndPlayerTurn;
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
        Entity player = playerTeam[0];
        
        InstantEffect instantEffect = new InstantEffect(InstantEffectSubType.Attack, 1);
        foreach (Entity enemy in enemyTeam)
        {
            player.ApplyEffect(instantEffect, enemy);
        }
    }

    private void EndPlayerTurn()
    {
        playerTurn = false;
        EnemyTeamPerformAction();
        playerTurn = true;
    }
}
