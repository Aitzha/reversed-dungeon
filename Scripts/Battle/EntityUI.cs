using Godot;
using System;

public partial class EntityUI : Node2D
{
    [Export] private ProgressBar healthBar;
    [Export] private Label healthLabel;
    [Export] private Label guardLabel;

    public void UpdateUI(EntityData entityData)
    {
        healthBar.MaxValue = entityData.maxHealth;
        healthBar.Value = entityData.health;
        healthLabel.Text = entityData.health + " / " + entityData.maxHealth;
        guardLabel.Text = entityData.guard.ToString();
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
