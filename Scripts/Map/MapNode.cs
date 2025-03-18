using Godot;
using System.Collections.Generic;

public partial class MapNode : Control
{
    [Export] private Sprite2D sprite;
    [Signal] public delegate void NodeClickedEventHandler(MapNode node);
    
    public int row;
    public int col;
    public MapNodeType type;
    public List<EntityData> enemies = new();
    public List<MapNode> children = new();
    

    public override void _Ready()
    {
        if (type == MapNodeType.Enemy)
        {
            enemies.Add((EntityData)ResourceLoader.Load<EntityData>("res://Data/Entities/Enemies/enemy#1.tres").Duplicate());
            enemies.Add((EntityData)ResourceLoader.Load<EntityData>("res://Data/Entities/Enemies/enemy#2.tres").Duplicate());
        }

        if (type == MapNodeType.EliteEnemy)
        {
            enemies.Add((EntityData)ResourceLoader.Load<EntityData>("res://Data/Entities/Enemies/enemy#1.tres").Duplicate());
            enemies.Add((EntityData)ResourceLoader.Load<EntityData>("res://Data/Entities/Enemies/enemy#2.tres").Duplicate());
            enemies.Add((EntityData)ResourceLoader.Load<EntityData>("res://Data/Entities/Enemies/enemy#3.tres").Duplicate());
        }

        if (type == MapNodeType.Boss)
        {
            enemies.Add((EntityData)ResourceLoader.Load<EntityData>("res://Data/Entities/Enemies/enemy#4.tres").Duplicate());
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
        sprite.Texture = GD.Load<Texture2D>("res://Sprites/UI/Map/Icons/" + Utils.ToSnakeCase(type) + ".png");
        sprite.Position = sprite.Texture.GetSize() / 2;
        Size = sprite.Texture.GetSize();
    }

    public void Select()
    {
        sprite.Texture = GD.Load<Texture2D>("res://Sprites/UI/Map/Icons/selected.png");
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


