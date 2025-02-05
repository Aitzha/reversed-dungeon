using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class MapNode : Control
{
    [Export] private Sprite2D sprite;
    [Signal] public delegate void NodeClickedEventHandler(MapNode node);
    
    public List<MapNode> children = new();
    public int row;
    public int col;
    public MapNodeType type;
    public List<EntityData> enemies = new();
    

    public override void _Ready()
    {
        if (type == MapNodeType.Enemy)
        {
            enemies.Add(new EntityData("Enemy#1", 20));
            enemies.Add(new EntityData("Enemy#2", 20));
        }

        if (type == MapNodeType.EliteEnemy)
        {
            enemies.Add(new EntityData("Enemy#1", 20));
            enemies.Add(new EntityData("Enemy#2", 20));
            enemies.Add(new EntityData("Enemy#3", 20));
        }

        if (type == MapNodeType.Boss)
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

    public void SetType(MapNodeType type)
    {
        this.type = type;
        sprite.Texture = GD.Load<Texture2D>("res://Sprites/UI/Map/Icons/" + type + ".png");
        sprite.Position = sprite.Texture.GetSize() / 2;
        Size = sprite.Texture.GetSize();
        
    }

    public void Select()
    {
        sprite.Texture = GD.Load<Texture2D>("res://Sprites/UI/Map/Icons/Selected.png");
        sprite.Position = sprite.Texture.GetSize() / 2;
        Size = sprite.Texture.GetSize();
    }

    public void Unselect()
    {
        SetType(type);
    }
}

public enum MapNodeType
{
    Start,
    Enemy,
    EliteEnemy,
    Boss
}
