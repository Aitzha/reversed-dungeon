using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Control
{
    [Export] private PackedScene startNodeScene;
    [Export] private PackedScene bossNodeScene;
    [Export] private PackedScene enemyNodeScene;
    [Export] private PackedScene eliteEnemyNodeScene;
    
    private Dictionary<MapNode, Dictionary<MapNode, Line2D>> linesDict = new();
    private MapNode curNode;
    
    public override void _Ready()
    {
        ChoosePaths(BuildMap());
    }

    private List<MapNode> BuildMap()
    {
        List<MapNode> nodes = new List<MapNode>();
        
        // mapRows must be odd number
        int mapRows = 7; 
        int mapCols = 8;
        int borderX = 50;
        int borderY = 60;
        int distX = (GameSettings.windowWidth - borderX * 2) / (mapCols - 1);
        int distY = (GameSettings.windowHeight - borderY * 2) / (mapRows - 1);
        
        Dictionary<int, Dictionary<int, MapNode>> nodesDict = new(); // <Row, Col> = Node
        for (int i = 0; i < mapRows; i++)
            nodesDict.Add(i, new Dictionary<int, MapNode>());
    
        MapNode startNode = (MapNode)startNodeScene.Instantiate();
        startNode.col = 0;
        startNode.row = (mapRows - 1) / 2;
        startNode.type = NodeType.Start;
        nodesDict[startNode.row].Add(startNode.col, startNode);
        nodes.Add(startNode);
        startNode.Position = new Vector2(borderX, borderY + distY * startNode.row) - startNode.Size / 2;

        Queue<MapNode> q = new Queue<MapNode>();
        q.Enqueue(startNode);
        while (q.Count > 0)
        {
            MapNode parent = q.Dequeue();

            // Don't ask me how I calculated it. Just made this formula and it works but I can't prove it :P
            int upperLimit = Math.Max(0, parent.col + 1 - (mapCols - 1 - (mapRows - 1) / 2));
            int lowerLimit = mapRows - 1 - upperLimit;
            for (int i = -1; i < 2; i++)
            {
                if (parent.row + i < upperLimit || parent.row + i > lowerLimit)
                    continue;
                
                if (nodesDict[parent.row + i].ContainsKey(parent.col + 1))
                {
                    parent.children.Add(nodesDict[parent.row + i][parent.col + 1]);
                }
                else
                {
                    // Instantiate node
                    MapNode child;
                    if (parent.col == mapCols - 2)
                    {
                        child = (MapNode) bossNodeScene.Instantiate();
                        child.type = NodeType.Boss;
                    }
                    else
                    {
                        float randNum = GD.Randf();
                        if (randNum < 0.95 - parent.col * 0.05)
                        {
                            child = (MapNode)enemyNodeScene.Instantiate();
                            child.type = NodeType.Enemy;
                        }
                        else
                        {
                            child = (MapNode)eliteEnemyNodeScene.Instantiate();
                            child.type = NodeType.EliteEnemy;
                        }
                    }
                    
                    // Setup node
                    child.col = parent.col + 1;
                    child.row = parent.row + i;
                    child.Position = new Vector2(borderX + distX * child.col, borderY + distY * child.row) - child.Size / 2;
                    
                    // Save child node
                    nodesDict[child.row].Add(child.col, child);
                    parent.children.Add(child);
                    nodes.Add(child);
                    
                    if (child.col < mapCols - 1)
                        q.Enqueue(child);
                }
            }
        }

        return nodes;
    }

    private void ChoosePaths(List<MapNode> nodes)
    {
        curNode = nodes[0];
        AddChild(nodes[0]);
        
        for (int i = 0; i < 5; i++)
        {
            MapNode parent = nodes[0];
            
            while (parent != null)
            {
                MapNode child = parent.children[GD.RandRange(0, parent.children.Count - 1)];
                CreateLine(parent, child);
                
                if (!GetChildren().Contains(child))
                    AddChild(child);
                
                if (child.children.Count > 0)
                    parent = child;
                else
                    parent = null;
            }
        }
    }

    private void CreateLine(MapNode parent, MapNode child)
    {
        Line2D line = new Line2D();
        line.Width = 3;
        line.DefaultColor = new Color(0, 0, 0);
        line.AddPoint(parent.GlobalPosition + parent.Size / 2);
        line.AddPoint(child.GlobalPosition + child.Size / 2);
        line.ZIndex = -1;
        AddChild(line);
    }
}
