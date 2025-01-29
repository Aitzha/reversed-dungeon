using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class CardHolder : Area2D
{
    public bool overTarget = false;
    private List<Entity> targetList = new List<Entity>();
    private Entity currentTarget = null;
    
    public override void _Ready()
    {
        Deactivate();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }
    
    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition();
    }

    public void Activate()
    {
        DisableMode = DisableModeEnum.Remove;
        SetProcess(true);
    }

    public void Deactivate()
    {
        GlobalPosition = new Vector2(-200, -200);
        DisableMode = DisableModeEnum.KeepActive;
        SetProcess(false);
    }

    private void OnBodyEntered(Node2D body)
    {
        Debug.Print("Entered body: " + body.Name);
        Entity entity = body as Entity;
        if (entity != null)
        {
            targetList.Add(entity);
            if (currentTarget == null)
            {
                overTarget = true;
                entity.ToggleGlow();
                currentTarget = entity;
            }
        }
    }

    private void OnBodyExited(Node2D body)
    {
        Debug.Print("Exited body: " + body.Name);
        Entity entity = body as Entity;
        if (entity != null)
        {
            targetList.Remove(entity);
            if (currentTarget == entity)
            {
                entity.ToggleGlow();
                if (targetList.Count > 0)
                {
                    currentTarget = targetList[0];
                    currentTarget.ToggleGlow();
                }
                else
                {
                    overTarget = false;
                    currentTarget = null;
                }
            }
        }
    }
}
