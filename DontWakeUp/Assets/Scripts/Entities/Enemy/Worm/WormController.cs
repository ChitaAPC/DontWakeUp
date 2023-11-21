using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormController : AbstractEntityController
{
    private Vector2 dir;
    private float idleDirChangeTimer;

    private float detectionRadious;

    private int layerMask;

    private float physical_bias;

    protected override void OnAwake()
    {
        InnitialiseProperties();
        InnitialiseMovement();
    }


    private void InnitialiseProperties()
    {
        EntityStats stats = new EntityStats();
        stats.maxHp = 10f;
        stats.movement_speed = 8f;
        stats.combat_speed = 4f;
        stats.attack_physical = 1f;
        stats.attack_emotional = 1f;
        stats.armour_physical = 1f;
        stats.armour_emotional = 1f;

        EntityStats buffs = new EntityStats();
        buffs.attack_emotional = -0.05f;
        buffs.maxHp = 2f;
        buffs.hp = 2f;


        physical_bias = Random.Range(0.3f, 0.7f);

        InnitialiseProperties(stats, buffs);
    }

    private void InnitialiseMovement()
    {
        layerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadious = 4.5f;

        UpdateIddleValues();
    }

    private void UpdateIddleValues()
    {

        idleDirChangeTimer = Random.Range(0.5f, 1f);
        if (Random.value > 0.3f)
        {
            if (Random.value > 0.5)
            {
                dir = new Vector2(Random.Range(-0.5f, 0.5f), 0f);
            }
            else
            {
                dir = new Vector2(0, Random.Range(-0.5f, 0.5f));
            }
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
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadious, layerMask);
        if (player == null)
        {
            DoIdleModeUpdate();
        }
        else
        {
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
        is_def_emotional = false;
        is_def_physical = false;


        if (Random.value > physical_bias)
        {
            if (Random.value < 0.5f)
            {
                //defend
                is_def_physical = true;
                return $"{gameObject.name} uses the ground as a barrier, it looks harder to hit.";
            }
            else
            {
                //attack
                player.TakeDamage(attack_physical, true);
                return $"{gameObject.name} smacks you";
            }
        }
        else
        {
            if (Random.value < 0.5f)
            {
                //defend
                is_def_emotional = true;
                return $"{gameObject.name} isn't listening to you, words are less effective";
            }
            else
            {
                player.TakeDamage(attack_emotional, false);
                return $"{gameObject.name} makes fun of you!";
            }
            
        }
    }
}