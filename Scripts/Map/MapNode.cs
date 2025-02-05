using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class MapNode : Control
{
    public List<MapNode> children = new();
    public int row;
    public int col;
    public NodeType type;
    public List<EntityData> enemies = new();

    [Signal] public delegate void NodeClickedEventHandler(MapNode node);

    public override void _Ready()
    {
        if (type == NodeType.Enemy)
        {
            enemies.Add(new EntityData("Enemy#1", 20));
            enemies.Add(new EntityData("Enemy#2", 20));
        }

        if (type == NodeType.EliteEnemy)
        {
            enemies.Add(new EntityData("Enemy#1", 20));
            enemies.Add(new EntityData("Enemy#2", 20));
            enemies.Add(new EntityData("Enemy#3", 20));
        }

        if (type == NodeType.Boss)
        {
            enemies.Add(new EntityData("Enemy#3", 50));
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.Pressed)
            {
                EmitSignal(SignalName.NodeClicked, this);
            }
        }
    }
}

public enum NodeType
{
    Start,
    Enemy,
    EliteEnemy,
    Boss
}
