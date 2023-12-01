using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwormSpiderController : AbstractEntityController
{
    protected override void OnAwake()
    {
        EntityStats stats = new EntityStats();
        stats.maxHp = 12f;
        stats.movement_speed = 0f;
        stats.combat_speed = 4f;
        stats.attack_physical = 4f;
        stats.attack_emotional = 2f;
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
        if (Random.value <= 0.1f)
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
