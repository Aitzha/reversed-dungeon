using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CardDatabase : Resource
{
    [Export] public Array<CardData> allCards = new();
}