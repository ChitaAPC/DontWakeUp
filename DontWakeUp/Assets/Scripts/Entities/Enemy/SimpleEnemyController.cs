using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyController : AbstractEntityController
{
    protected override void OnAwake()
    {
        InnitialiseProperties();
    }

    private void InnitialiseProperties()
    {
        float hp = 10f;
        float movement_speed = 5f;
        float attack_physical = 5f;
        float attack_emotional = 5f;

        float armour_physical = 1f;
        float armour_emotional = 1f;

        InnitialiseProperties(hp, movement_speed, attack_physical, attack_emotional, armour_physical, armour_emotional);
    }

}
