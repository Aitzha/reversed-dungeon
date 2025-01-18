using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Node
{
    [Export] public BattleInterface BattleInterface;
    
    public List<Entity> playerTeam = new List<Entity>();
    public List<Entity> enemyTeam = new List<Entity>();

    public void Setup(List<CardData> playerCards, List<Entity> playerTeam, List<Entity> enemyTeam)
    {
        BattleInterface.Setup(playerCards);
        this.playerTeam = playerTeam;
        this.enemyTeam = enemyTeam;
    }
    
    
}
