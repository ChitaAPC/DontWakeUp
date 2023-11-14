using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AbstractEntityController
{
    private Vector2 dir;
    private Animator animator;

    protected override void OnAwake()
    {
        InnitialiseMovmeent();
        InnitialiseProperties();
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void InnitialiseProperties()
    {
        EntityStats stats = new EntityStats();
        stats.maxHp = 10f;
        stats.movement_speed = 5f;
        stats.combat_speed = 5f;
        stats.attack_physical = 5f;
        stats.attack_emotional = 5f;

        stats.armour_physical = 0f;
        stats.armour_emotional = 0f;
        
        InnitialiseProperties(stats, new EntityStats());
    }

    private void InnitialiseMovmeent()
    {
        dir = new Vector2(0f, 0f);
    }

    private void Update()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirY = Input.GetAxisRaw("Vertical");
        dir = new Vector2(moveDirX, moveDirY);
        animator.SetFloat("Horizontal", moveDirX);
        animator.SetFloat("Vertical", moveDirY);
        animator.SetFloat("Speed", dir.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        MoveEntityOnDirection(dir);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        AbstractEntityController enemy = collision.gameObject.GetComponent<AbstractEntityController>();

        if (enemy != null)
        {
            EventHandler.instance.BattleStartEvent.Invoke(this, enemy);
        }
    }

    public override string DoAIAction(AbstractEntityController player)
    {
        //player should never be controlled by the AI
        throw new System.NotImplementedException();
    }
}
