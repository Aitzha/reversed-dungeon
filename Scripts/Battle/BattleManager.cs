using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Node
{
    [Export] public BattleInterface BattleInterface;

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

    public void Setup(List<CardData> playerCards, List<Entity> playerTeam, List<Entity> enemyTeam)
    {
        BattleInterface.playerCards = playerCards;
        this.playerTeam = playerTeam;
        this.enemyTeam = enemyTeam;
    }

    public override void _Ready()
    {
        LoadEntities();
    }

    private void LoadEntities()
    {
        int playerCount = 0;
        foreach (Entity player in playerTeam) 
        {
            PackedScene packedPlayerScene = ResourceLoader.Load<PackedScene>("res://Scenes/BattleScenes/Characters/Player/" + player.EntityName + ".tscn");

            var e = (Entity)packedPlayerScene.Instantiate();
            e.CopyOriginal(player);
            AddChild(e);
            
            e.Position = playerTeamPositions[playerCount];
            e.Position = playerTeamPositions[playerCount];
            playerCount++;
        }

        int enemyCount = 0;
        foreach (Entity enemy in enemyTeam) 
        {
            PackedScene packedEnemyScene = ResourceLoader.Load<PackedScene>("res://Scenes/BattleScenes/Characters/Enemies/" + enemy.EntityName + ".tscn");

            var e = (Entity)packedEnemyScene.Instantiate();
            e.CopyOriginal(enemy);
            AddChild(e);

            e.Position = enemyTeamPositions[enemyCount];
            enemyCount++;
        }
    }
}
