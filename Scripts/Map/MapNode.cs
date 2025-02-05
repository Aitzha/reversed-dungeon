using Godot;
using System;
using System.Collections.Generic;

public partial class MapNode : Control
{
    public List<MapNode> children = new();
    public int row;
    public int col;
}
