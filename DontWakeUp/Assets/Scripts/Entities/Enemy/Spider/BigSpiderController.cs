using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSpiderController : AbstractEntityController
{
    
    protected override void OnAwake()
    {
        EntityStats stats = new EntityStats();
        stats.maxHp = 2f;
        stats.movement_speed = 0f;
        stats.combat_speed = 6f;
        stats.attack_physical = 6f;
        stats.attack_emotional = 3f;
        stats.armour_physical = 2f;
        stats.armour_emotional = 2f;

        EntityStats buffs = new EntityStats();
        buffs.combat_speed = 0.1f;
        buffs.attack_physical = -0.05f;

        InnitialiseProperties(stats, buffs);
    }

    public override void OnBattleStartLogic()
    {
        animator.SetTrigger("combat");
    }


    public override string DoAIAction(AbstractEntityController player)
    {
        if (Random.value <= 0.9f)
        {
            player.TakeDamage(attack_physical, true);
            return $"{transform.name} bites you";
        }
        else
        {
            player.TakeDamage(attack_emotional, false);
            return $"{transform.name} intimidates you";
        }
    }
}
