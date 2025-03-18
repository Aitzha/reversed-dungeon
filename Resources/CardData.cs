using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CardData : Resource
{
    [Export] public string id { get; set; }
    [Export] public string name { get; set; }
    [Export] public string descriptionTemplate { get; set; }
    [Export] public int cost { get; set; } = 1;
    [Export] public int tier { get; set; } = 0;
    [Export] public int progress { get; set; } = 0;
    [Export] public TargetType targetType { get; set; }
    [Export] public TargetFaction targetFaction { get; set; }

    [Export] public Array<BaseEffect> effects { get; set; } = new();

    public string GetDescription()
    {
        return Regex.Replace(descriptionTemplate, @"\{(\w+)_(\d+)\}", match =>
        {
            string key = match.Groups[1].Value;
            int effectIndex = int.Parse(match.Groups[2].Value);
			
            BaseEffect effect = effects[effectIndex];
            if (key == "amount") return effect.magnitude.ToString();
            if (key == "duration") return effect.duration.ToString();

            // Return placeholder if no match found
            return match.Value;
        });
    }
}

[GlobalClass]
public partial class BaseEffect : Resource
{
    [Export] public int duration = 0;
    [Export] public int magnitude = 0;
    public virtual bool IsStatusEffect { get; } = false;
    public virtual EffectType type { get; } = EffectType.Attack;
    public Entity target;
    public Entity caster;
    protected BattleFX fx;
    
    
    public virtual void ApplyEffect() {}
    public virtual BaseEffect Clone(Entity target, Entity caster) {return new BaseEffect();}
}

public enum EffectType
{
    Attack,
    Bleed,
    Heal,
    Guard,
    Buff,
    Debuff
}

public enum TargetType
{
    Single,
    Group
}

public enum TargetFaction
{
    Self,
    Ally,
    Enemy
}

