using Godot;
using System;
using System.Diagnostics;

public partial class EntityUI : Node2D
{
    [Export] private ProgressBar healthBar;
    [Export] private Label healthLabel;
    [Export] private Label guardLabel;
    [Export] public HBoxContainer statusEffectList;
    [Export] public PackedScene statusEffectScene;

    public void UpdateUI(Entity entity)
    {
        healthBar.MaxValue = entity.entityData.maxHealth;
        healthBar.Value = entity.entityData.health;
        healthLabel.Text = entity.entityData.health + " / " + entity.entityData.maxHealth;
        guardLabel.Text = entity.guard.ToString();
        
        foreach (Node node in statusEffectList.GetChildren())
            node.QueueFree();
        
        statusEffectList.GetChildren().Clear();

        foreach (BaseEffect statusEffect in entity.statusEffects)
        {
            Node node = statusEffectScene.Instantiate();
            node.GetNode<Sprite2D>("Sprite").Texture = GD.Load<Texture2D>("res://Sprites/UI/Battle/" + Utils.ToSnakeCase(statusEffect.GetType()) + ".png");
            node.GetNode<Label>("Label").Text = statusEffect.duration.ToString();
            statusEffectList.AddChild(node);
        }
    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBar.MaxValue = maxHealth;
        healthBar.Value = currentHealth;
        healthLabel.Text = currentHealth + " / " + maxHealth;
    }

    public void UpdateGuard(int currentGuard)
    {
        guardLabel.Text = currentGuard.ToString();
    }
}
