using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSlimeController : AbstractEntityController
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
        stats.maxHp = 10f;
        stats.movement_speed = 4f;
        stats.combat_speed = 3f;
        stats.attack_physical = 3f;
        stats.attack_emotional = 0f;
        stats.armour_physical = 0f;
        stats.armour_emotional = 0f;

        EntityStats buffs = new EntityStats();
        buffs.armour_physical = -0.1f;
        buffs.attack_emotional = 0.1f;

        InnitialiseProperties(stats, buffs);
    }

    private void InnitialiseMovement()
    {
        detectionRadious = 3.5f;

        UpdateIddleValues();
    }

    private void UpdateIddleValues()
    {
        idleDirChangeTimer = Random.Range(0.5f, 5f);
        if (Random.value > 0.3f)
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
        animator.SetFloat("Horizontal", dir.x * Time.timeScale);
        animator.SetFloat("Vertical", dir.y * Time.timeScale);
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
            animator.SetBool("Angry", false);
            DoIdleModeUpdate();
        }
        else
        {
            animator.SetBool("Angry", true);
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
        player.TakeDamage(attack_physical, true);
        return $"{gameObject.name} just physically hit you!";
    }
}
