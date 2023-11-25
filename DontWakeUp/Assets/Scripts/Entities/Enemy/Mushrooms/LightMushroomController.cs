using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMushroomController : AbstractEntityController
{
    private Vector2 dir;
    private float idleDirChangeTimer;


    protected override void OnAwake()
    {
        InnitialiseProperties();
        InnitialiseMovement();
    }


    private void InnitialiseProperties()
    {
        EntityStats stats = new EntityStats();
        stats.maxHp = 6f;
        stats.movement_speed = 2f;
        stats.combat_speed = 6f;
        stats.attack_physical = 0f;
        stats.attack_emotional = 3f;
        stats.armour_physical = 0f;
        stats.armour_emotional = 3f;

        EntityStats buffs = new EntityStats();
        buffs.combat_speed = -0.05f;
        buffs.armour_emotional = 0.1f;

        InnitialiseProperties(stats, buffs);
    }

    private void InnitialiseMovement()
    {
        detectionRadious = 4.5f;

        UpdateIddleValues();
    }

    private void UpdateIddleValues()
    {
        idleDirChangeTimer = Random.Range(0.5f, 1f);
        if (Random.value > 0.15f)
        {

            dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        }
        else
        {
            dir = new Vector2(0f, 0f);
        }
    }


    private void Update()
    {
        animator.SetFloat("Speed", dir.sqrMagnitude * Time.timeScale);
    }


    private void FixedUpdate()
    {
        HandleMovementMode();
    }

    private void HandleMovementMode()
    {
        if (IsIdleMode())
        {
            DoIdleModeUpdate();
        }
        else
        {
            DoAttackModeUpdate();
        }
    }

    private void DoIdleModeUpdate()
    {
        MoveEntityOnDirection(dir, false);
        idleDirChangeTimer -= Time.deltaTime;
        if (idleDirChangeTimer <= 0f)
        {
            UpdateIddleValues();
        }
    }

    private void DoAttackModeUpdate()
    {
        dir = player.transform.position - transform.position;
        MoveEntityOnDirection(dir);
    }

    public override string DoAIAction(AbstractEntityController player)
    {
        if (is_def_emotional)
        {
            is_def_emotional = false;
        }

        if (Random.value < 0.5f)
        {
            //defend
            is_def_emotional = true;
            return $"{gameObject.name} takes a deep breath and smiles. It looks more confident in itself...";
        }
        else
        {
            player.TakeDamage(attack_emotional, false);
            return $"{gameObject.name} was mean to you!";
        }
    }
}
