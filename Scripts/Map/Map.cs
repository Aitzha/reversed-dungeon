using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Map : Control
{
    [Export] private PackedScene mapNodeScene;

    [Signal] public delegate void NodeClickedEventHandler(MapNode node);
    
    // mapRows must be odd number
    public int mapRows = 7; 
    public int mapCols = 8;
    
    private MapNode curNode;
    
    public override void _Ready()
    {
        ChoosePaths(BuildMap());
    }

    private void OnNodeClick(MapNode node)
    {
        if (curNode.children.Contains(node))
        {
            curNode.Unselect();
            node.Select();
            RepositionMapNode(curNode);
            RepositionMapNode(node);
            curNode = node;

            EmitSignal(SignalName.NodeClicked, curNode);
        }
    }
    
    private List<MapNode> BuildMap()
    {
        List<MapNode> nodes = new List<MapNode>();
        
        Dictionary<int, Dictionary<int, MapNode>> nodesDict = new(); // <Row, Col> = Node
        for (int i = 0; i < mapRows; i++)
            nodesDict.Add(i, new Dictionary<int, MapNode>());
    
        // Create start node
        MapNode startNode = CreateMapNode(0, (mapRows - 1) / 2, MapNodeType.Start);
        nodesDict[startNode.row].Add(startNode.col, startNode);
        nodes.Add(startNode);

        // Start map creation from first node
        Queue<MapNode> q = new Queue<MapNode>();
        q.Enqueue(startNode);
        while (q.Count > 0)
        {
            MapNode parent = q.Dequeue();

            // Calculate upper and lower limit for child nodes
            int upperLimit = Math.Max(0, parent.col + 1 - (mapCols - 1 - (mapRows - 1) / 2));
            int lowerLimit = mapRows - 1 - upperLimit;
            for (int i = -1; i < 2; i++)
            {
                if (parent.row + i < upperLimit || parent.row + i > lowerLimit)
                    continue;
                
                // if node already exist save it to parent
                if (nodesDict[parent.row + i].ContainsKey(parent.col + 1))
                {
                    parent.children.Add(nodesDict[parent.row + i][parent.col + 1]);
                }
                else
                {
                    // Randomly choose node type
                    MapNodeType type;
                    if (parent.col == mapCols - 2)
                        type = MapNodeType.Boss;
                    else
                    {
                        float randNum = GD.Randf();
                        if (randNum < 0.95 - parent.col * 0.05)
                            type = MapNodeType.Enemy;
                        else
                            type = MapNodeType.EliteEnemy;
                    }

                    // Create, Setup and Save new node
                    MapNode child = CreateMapNode(parent.col + 1, parent.row + i, type);
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

    private MapNode CreateMapNode(int col, int row, MapNodeType type)
    {
        MapNode node = (MapNode)mapNodeScene.Instantiate();
        node.col = col;
        node.row = row;
        node.SetType(type);
        RepositionMapNode(node);

        return node;
    }

    private void RepositionMapNode(MapNode node)
    {
        int borderX = 50;
        int borderY = 60;
        int distX = (GameSettings.windowWidth - borderX * 2) / (mapCols - 1);
        int distY = (GameSettings.windowHeight - borderY * 2) / (mapRows - 1);
        node.Position = new Vector2(borderX + distX * node.col, borderY + distY * node.row) - node.Size / 2;
    }

    private void ChoosePaths(List<MapNode> nodes)
    { 
        Dictionary<MapNode, List<MapNode>> newChildren = new();
        
        curNode = nodes[0];
        curNode.Select();
        AddMapNode(nodes[0]);
        
        for (int i = 0; i < 5; i++)
        {
            MapNode parent = nodes[0];
            
            while (parent != null)
            {
                if (!newChildren.ContainsKey(parent))
                    newChildren.Add(parent, new List<MapNode>());
                
                MapNode child = parent.children[GD.RandRange(0, parent.children.Count - 1)];
                CreateLine(parent, child);
                newChildren[parent].Add(child);
                
                if (!GetChildren().Contains(child))
                    AddMapNode(child);
                
                if (child.children.Count > 0)
                    parent = child;
                else
                    parent = null;
            }
        }

        foreach (MapNode parent in GetChildren().OfType<MapNode>())
            if (newChildren.ContainsKey(parent))
                parent.children = newChildren[parent];
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

    private void AddMapNode(MapNode node)
    {
        AddChild(node);
        node.NodeClicked += OnNodeClick;
    }
}
