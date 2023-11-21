using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeethController : AbstractEntityController
{

    protected override void OnAwake()
    {
        InnitialiseProperties();
    }


    private void InnitialiseProperties()
    {
        EntityStats stats = new EntityStats();
        stats.maxHp = 20f;
        stats.movement_speed = 0f;
        stats.combat_speed = 7f;
        stats.attack_physical = 5f;
        stats.attack_emotional = 5f;
        stats.armour_physical = 3f;
        stats.armour_emotional = 3f;

        EntityStats buffs = new EntityStats();

        InnitialiseProperties(stats, buffs);
    }

    public override string DoAIAction(AbstractEntityController player)
    {
        is_def_emotional = false;
        is_def_physical = false;


        if (Random.value > 0.5f)
        {
            //physical
            if (Random.value < 0.5f)
            {
                //defend
                is_def_physical = true;
                return $"The {gameObject.name} have more plaque now, it is creating a protective barrier";
            }
            else
            {
                //attack
                player.TakeDamage(attack_physical, true);
                return $"{gameObject.name} bites you";
            }
        }
        else
        {
            //emotional
            if (Random.value < 0.5f)
            {
                //defend
                is_def_emotional = true;
                return $"{gameObject.name} smiles proudly, it looks more confident somehow";
            }
            else
            {
                player.TakeDamage(attack_emotional, false);
                return $"{gameObject.name} laughs at you mockingly...";
            }

        }
    }
}
