using Godot;
using System;
using System.Text.Json.Serialization;


// public class Effect 
// {
// 	public EffectType type { get; set; }
// 	public EffectSubtype subtype { get; set; }
// 	public FirstTriggerTiming firstTriggerTiming { get; set; }
// 	public DurationReductionTiming durationReductionTiming { get; set; }
// 	public int duration { get; set; } = 1;
// 	public int amount { get; set; }
// 	public Entity target;
// 	public Entity caster;
//
// 	public Effect() {}
//
// 	public Effect(EffectType type, EffectSubtype subtype, FirstTriggerTiming firstTriggerTiming, 
// 		DurationReductionTiming durationReductionTiming, int duration, int amount)
// 	{
// 		this.type = type;
// 		this.subtype = subtype;
// 		this.firstTriggerTiming = firstTriggerTiming;
// 		this.durationReductionTiming = durationReductionTiming;
// 		this.duration = duration;
// 		this.amount = amount;
// 	}
// 	
// 	public Effect(EffectType type, EffectSubtype subtype, FirstTriggerTiming firstTriggerTiming, 
// 		DurationReductionTiming durationReductionTiming, int duration, int amount, Entity target, Entity caster)
// 	{
// 		this.type = type;
// 		this.subtype = subtype;
// 		this.firstTriggerTiming = firstTriggerTiming;
// 		this.durationReductionTiming = durationReductionTiming;
// 		this.amount = amount;
// 		this.duration = duration;
// 		this.target = target;
// 		this.caster = caster;
// 	}
// 	
// 	public void ApplyEffect()
// 	{
// 		switch (type)
// 		{
// 			case EffectType.Attack:
// 				if (subtype is EffectSubtype.None)
// 				{
// 					int casterAttack = Math.Max(0, amount + caster.entityData.attackPower);
// 					int damageOnGuard = Math.Min(target.entityData.guard, casterAttack);
// 					target.entityData.guard -= damageOnGuard;
// 					target.entityData.health -= (casterAttack - damageOnGuard);
// 				}
//
// 				if (subtype is EffectSubtype.Bleed)
// 				{
// 					target.entityData.health -= amount;
// 					target.statusEffects.Add(new StatusEffect(duration, StatusEffectType.Bleed));
// 				}
// 				
// 				if (subtype is EffectSubtype.Poison)
// 				{
// 					target.entityData.health -= amount;
// 					target.statusEffects.Add(new StatusEffect(duration, StatusEffectType.Poison));
// 				}
// 				
// 				break;
// 			case EffectType.Guard:
// 				target.entityData.guard += amount;
// 				break;
// 			case EffectType.Heal:
// 				target.entityData.health += Math.Min(target.entityData.health + amount, target.entityData.maxHealth);
// 				
// 				if (subtype is EffectSubtype.Regeneration)
// 					target.statusEffects.Add(new StatusEffect(duration, StatusEffectType.Regeneration));
// 				
// 				break;
// 			case EffectType.StatusEffect:
// 				if (subtype is EffectSubtype.AttackBuff)
// 				{
// 					target.entityData.attackPower += amount;
// 					target.statusEffects.Add(new StatusEffect(duration, StatusEffectType.AttackBuff));
// 				}
//
// 				if (subtype is EffectSubtype.AttackDebuff)
// 				{
// 					target.entityData.attackPower -= amount;
// 					target.statusEffects.Add(new StatusEffect(duration, StatusEffectType.AttackDebuff));
// 				}
//
// 				if (subtype is EffectSubtype.Paralyze)
// 				{
// 					target.entityData.isParalyzed = true;
// 				}
// 				break;
// 		}
// 		
// 		if (durationReductionTiming == DurationReductionTiming.OnEffectApply)
// 			duration--;
// 	}
//
// 	public Effect Clone(Entity target, Entity caster)
// 	{
// 		return new Effect(type, subtype, firstTriggerTiming, durationReductionTiming, duration, amount, target, caster);
// 	}
// }

