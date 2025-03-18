using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EntityData : Resource
{ 
    [Export] public string entityName { get; set; }
    [Export] public int health { get; set; }
    [Export] public int maxHealth { get; set; }
    [Export] public Array<BaseEffect> possibleActions { get; set; } = new();
    
    public EntityData(string entityName, int maxHealth)
    {
        this.entityName = entityName;
        this.maxHealth = maxHealth;
        health = maxHealth;
    }

    public EntityData() {}
}
