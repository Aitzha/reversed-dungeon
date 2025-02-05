using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Control
{
    [Export] private PackedScene startNodeScene;
    [Export] private PackedScene bossNodeScene;
    [Export] private PackedScene enemyNodeScene;
    [Export] private PackedScene eliteEnemyNodeScene;

    private int mapRows = 3;
    private int mapCols = 6;
    private int borderX = 50;
    private int borderY = 60;
    private int distX;
    private int distY;

    private Dictionary<int, Dictionary<int, MapNode>> nodesDict = new();
    
    public override void _Ready()
    {
        distX = (GameSettings.windowWidth - borderX * 2) / (mapCols - 1);
        distY = (GameSettings.windowHeight - borderY * 2) / (mapRows - 1);
        
        for (int i = 0; i < mapRows; i++)
            nodesDict.Add(i, new Dictionary<int, MapNode>());
        
        BuildMap();
    }

    private void BuildMap()
    {
        MapNode startNode = (MapNode)startNodeScene.Instantiate();
        startNode.col = 0;
        startNode.row = (mapRows - 1) / 2;
        nodesDict[startNode.row].Add(startNode.col, startNode);
        AddChild(startNode);
        startNode.Position = new Vector2(borderX - startNode.Size.X / 2, borderY + distY * startNode.row - startNode.Size.Y / 2);;

        Queue<MapNode> q = new Queue<MapNode>();
        q.Enqueue(startNode);
        while (q.Count > 0)
        {
            MapNode parent = q.Dequeue();

            if (parent.col == mapCols - 2)
            {
                if (nodesDict[startNode.row].ContainsKey(parent.col + 1))
                {
                    parent.children.Add(nodesDict[startNode.row][parent.col + 1]);
                }
                else
                {
                    MapNode bossNode = (MapNode) bossNodeScene.Instantiate();
                    AddChildNode(parent, startNode.row, bossNode);
                }
                continue;
            }
            
            for (int i = -1; i < 2; i++)
            {
                if (parent.row + i < 0 || parent.row + i > mapRows - 1)
                    continue;

                if (nodesDict[parent.row + i].ContainsKey(parent.col + 1))
                {
                    parent.children.Add(nodesDict[parent.row + i][parent.col + 1]);
                }
                else
                {
                    MapNode child = (MapNode) enemyNodeScene.Instantiate();
                    AddChildNode(parent, parent.row + i, child);
                    q.Enqueue(child);
                }
            }
        }
    }

    private void AddChildNode(MapNode parent, int row, MapNode child)
    {
        child.col = parent.col + 1;
        child.row = row;
        nodesDict[child.row].Add(child.col, child);
        parent.children.Add(child);
        AddChild(child);
        Vector2 offset = -child.Size / 2;
        child.Position = new Vector2(borderX + distX * child.col, borderY + distY * child.row) + offset;
    }
}
