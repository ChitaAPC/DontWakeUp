using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyController : AbstractEntityController
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
        float hp = 10f;
        
        float movement_speed = 4f;
        float combat_speed = 3f;

        float attack_physical = 3f;
        float attack_emotional = 0f;

        float armour_physical = 0f;
        float armour_emotional = 0f;

        InnitialiseProperties(hp, movement_speed, combat_speed, attack_physical, attack_emotional, armour_physical, armour_emotional);
    }

    private void InnitialiseMovement()
    {
        layerMask = 1 << LayerMask.NameToLayer("Player");
        detectionRadious = 3.5f;

        UpdateIddleValues();
    }

    private void UpdateIddleValues()
    {
        idleDirChangeTimer = Random.Range(0.5f, 5f);
        dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
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
        MoveEntityOnDirection(player.position - transform.position);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(-1f, -1f)).normalized * detectionRadious,Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(0, -1f)).normalized * detectionRadious, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(-1f, 0)).normalized * detectionRadious, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(1f, 1f)).normalized * detectionRadious, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(0f, 1f)).normalized * detectionRadious, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(1f, 0f)).normalized * detectionRadious, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(-1f, 1f)).normalized * detectionRadious, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector2(1f, -1f)).normalized * detectionRadious, Color.red);
    }

    public override string DoAIAction(AbstractEntityController player)
    {
        player.TakeDamage(attack_physical, true);
        return $"{gameObject.name} just physically hit you!";
    }

}
