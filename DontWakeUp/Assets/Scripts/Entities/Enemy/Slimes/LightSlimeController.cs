using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSlimeController : AbstractEntityController
{

    private Vector2 dir;
    private float idleDirChangeTimer;

    private float detectionRadious;

    private int layerMask;

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
        stats.attack_physical = 0f;
        stats.attack_emotional = 3f;
        stats.armour_physical = 0f;
        stats.armour_emotional = 0f;

        EntityStats buffs = new EntityStats();
        buffs.armour_emotional = -0.1f;
        buffs.attack_physical = 0.1f;

        InnitialiseProperties(stats, buffs);
    }

    private void InnitialiseMovement()
    {
        layerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadious = 4f;

        UpdateIddleValues();
    }

    private void UpdateIddleValues()
    {
        idleDirChangeTimer = Random.Range(0.5f, 5f);
        if (Random.value > 0.7f)
        {
            dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        }
        else
        {
            dir = new Vector2(0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        HandleMovementMode();
    }

    private void HandleMovementMode()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadious, layerMask);
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
        animator.SetFloat("Speed", dir.sqrMagnitude);
        if (player == null)
        {
            animator.SetBool("Angry", false);
            DoIdleModeUpdate();
        }
        else
        {
            animator.SetBool("Angry", true);
            DoAttackModeUpdate(player.transform);
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

    private void DoAttackModeUpdate(Transform player)
    {
        dir = player.position - transform.position;
        MoveEntityOnDirection(dir);
    }

    public override string DoAIAction(AbstractEntityController player)
    {
        player.TakeDamage(attack_emotional, false);
        return $"{gameObject.name} just hurt your feelings";
    }
}