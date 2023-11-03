using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    private float movement_speed;

    private float basic_attack_physical;
    private float basic_attack_emotional;

    private float armour_physical;
    private float armour_emotional;


    private float moveDirX;
    private float moveDirY;

    private Rigidbody2D rd;

    private void Awake()
    {
        InnitialiseProperties();
        InnitialiseMovmeent();
    }

    private void Start()
    {
        InnitialiseComponents();
    }

    private void InnitialiseProperties()
    {
        movement_speed = 5f;
        basic_attack_physical = 5f;
        basic_attack_emotional = 5f;

        armour_physical = 1f;
        armour_emotional = 1f;
    }

    private void InnitialiseMovmeent()
    {
        moveDirX = 0f;
        moveDirY = 0f;
    }

    private void InnitialiseComponents()
    {
        rd = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDirX = Input.GetAxis("Horizontal");
        moveDirY = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        rd.MovePosition(rd.position + (new Vector2(moveDirX, moveDirY).normalized * movement_speed * Time.deltaTime));
    }
}
