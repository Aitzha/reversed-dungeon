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
    private List<Entity> enemyTeam = new();
    private Queue<Entity> turnQueue = new();
    private int enemyCount = 0;

    private Vector2 playerPos = new(110, 220);
    private List<Vector2> enemyTeamPos = new()
    {
        new Vector2(465, 205),
        new Vector2(570, 270),
        new Vector2(360, 140),
    };

    

    public void Setup(Array<CardData> playerCards, EntityData playerData, List<EntityData> enemyTeamData)
    {
        instance = this;
        battleInterface.playerCards = playerCards;
        
        PackedScene packedPlayerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Battle/Characters/Player/" + playerData.entityName + ".tscn");
        player = (Entity)packedPlayerScene.Instantiate();
        player.entityData = playerData;
        player.Position = playerPos;
        player.isPlayerAlly = true;

        enemyCount = enemyTeamData.Count;
        for (int i = 0; i < enemyCount; i++) 
        {
            PackedScene packedEnemyScene = ResourceLoader.Load<PackedScene>("res://Scenes/Battle/Characters/Enemies/" + enemyTeamData[i].entityName + ".tscn");

            Entity enemyTeamMember = (Entity)packedEnemyScene.Instantiate();
            enemyTeamMember.entityData = enemyTeamData[i];

            enemyTeamMember.Position = enemyTeamPos[i];
            enemyTeamMember.isPlayerAlly = false;
            enemyTeam.Add(enemyTeamMember);
        }
    }

    public override void _Ready()
    {
        AddChild(player);
        turnQueue.Enqueue(player);

        foreach (Entity enemyTeamMember in enemyTeam)
        {
            AddChild(enemyTeamMember);
            turnQueue.Enqueue(enemyTeamMember);
        }
        
        StartBattle();
    }
    
    public async void StartBattle()
    {
        while (turnQueue.Count > 0)
        {
            Entity entity = turnQueue.Dequeue();
            if (entity.isDead)
            {
                DestroyEntity(entity);
                continue;
            }
            
            await entity.StartTurn();
            
            if (entity.isDead)
            {
                DestroyEntity(entity);
                continue;
            }

            if (entity == player)
            {
                playerMana = playerManaCapacity;
                battleInterface.FillHand();
                await ToSignal(battleInterface, nameof(BattleInterface.PlayerEndedTurn));
            }
            else
            {
                if (entity.isPlayerAlly)
                    await entity.PerformAction(enemyTeam);
                else
                    await entity.PerformAction(new List<Entity> {player});
            }
            
            entity.FinishTurn();
            turnQueue.Enqueue(entity);
        }
    }
    

    public void KillEntity(Entity entity)
    {
        if (entity.entityData.health > 0)
            return;
        
        entity.isDead = true;
        
        Tween tween;
        tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Linear);
        tween.TweenProperty(entity, "modulate", new Color(1f, 1f, 1f, 0.0f), 1.0f);
        
        if (entity == player)
        {
            Node endScreenInstance = endScreen.Instantiate();
            endScreenInstance.GetNode<Label>("Text").Text = "You Lose";
            AddChild(endScreenInstance);
        }
        else
        {
            enemyCount--;

            if (enemyCount == 0)
            {
                EmitSignal(SignalName.BattleWon);
            }
        }
    }

    private void DestroyEntity(Entity entity)
    {
        if (!entity.isDead)
            return;

        if (!entity.isPlayerAlly)
        {
            RemoveChild(entity);
            enemyTeam.Remove(entity);
            entity.QueueFree();
        }
    }
}
