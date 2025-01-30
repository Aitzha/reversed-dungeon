using Godot;
using System;
using System.Diagnostics;

public partial class BattleEventBus : Node
{
    public static BattleEventBus instance;
    
    [Signal]
    public delegate void EndTurnEventHandler();

    public override void _Ready()
    {
        instance = this;
    }

    public void OnEndTurn()
    {
        EmitSignal(SignalName.EndTurn);
    }
}
