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
    public int guard { get; set; }
    public int attackPower { get; set; }
    public bool isParalyzed { get; set; }
    public List<BaseEffect> statusEffects { get; set; } = new();
    public List<BaseEffect> effectsInAction { get; set; } = new();
    
    public EntityData(string entityName, int maxHealth)
    {
        this.entityName = entityName;
        this.maxHealth = maxHealth;
        health = maxHealth;
        guard = 0;
        attackPower = 0;
        isParalyzed = false;
        statusEffects = new List<BaseEffect>();
        effectsInAction = new List<BaseEffect>();
    }

    public EntityData() {}
}
