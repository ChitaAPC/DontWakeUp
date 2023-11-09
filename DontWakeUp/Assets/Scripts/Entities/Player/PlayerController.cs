using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AbstractEntityController
{
    private float moveDirX;
    private float moveDirY;

    protected override void OnAwake()
    {
        InnitialiseMovmeent();
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

    private void InnitialiseMovmeent()
    {
        moveDirX = 0f;
        moveDirY = 0f;
    }

    private void Update()
    {
        moveDirX = Input.GetAxisRaw("Horizontal");
        moveDirY = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        MoveEntityOnDirection(new Vector2(moveDirX, moveDirY));
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        AbstractEntityController enemy = collision.gameObject.GetComponent<AbstractEntityController>();

        if (enemy != null)
        {
            EventHandler.instance.BattleStartEvent.Invoke(this, enemy);
        }
    }

}
