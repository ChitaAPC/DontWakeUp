using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class AbstractEntityController : MonoBehaviour
{


    public float hp { private set; get; }
    public float maxHp { private set; get; }

    public float movement_speed { private set; get; }

    public float attack_physical { private set; get; }
    public float attack_emotional { private set; get; }

    public float armour_physical { private set; get; }
    public float armour_emotional { private set; get; }


    private Rigidbody2D rb;


    private bool innitialised;

    private void Awake()
    {
        innitialised = false;
        rb = GetComponent<Rigidbody2D>();

        OnAwake();
        Debug.Assert(innitialised, "ERROR: you must call \"InnitialiseProperties\" during OnAwake to initialise all of the values for this entity");
    }

    protected abstract void OnAwake();

    protected void InnitialiseProperties(float maxHp, float movement_speed, float attack_physical, float attack_emotional,
        float armour_physical, float armour_emotional)
    {
        hp = maxHp;
        this.maxHp = maxHp;
        this.movement_speed = movement_speed;
        this.attack_physical = attack_physical;
        this.attack_emotional = attack_emotional;
        this.armour_physical = armour_physical;
        this.armour_emotional = armour_emotional;

        rb.gravityScale = 0f;
        innitialised = true;
    }

    /// <summary>
    /// Returns the health as a fraction percentage of the max health.
    /// This means that this function will return 1 when the entity is at max health and 0 when the entity is at no health
    /// </summary>
    /// <returns></returns>
    public float GetHpPercent()
    {
        return hp / maxHp;
    }


    /// <summary>
    /// The entity will take <code>dmg</code> amount of damage.
    /// The function will then return True if it is dead after taking damage, False otherwise.
    /// </summary>
    /// <param name="dmg">The ammount of damage to be taken by the entity</param>
    /// <returns>True if the new HP is bellow or equal to zero, False otherwise.</returns>
    public bool TakeDamage(float dmg)
    {
        hp -= dmg;
        hp = Mathf.Max(0f, hp);
        return hp <= 0f;
    }

    /// <summary>
    /// Safe Heal: it will apply heal to the entity up to and including its maximum HP.
    /// This is a safe heal which will never leave the entity on overhealth (i.e. with more HP than its Max HP)
    /// </summary>
    /// <param name="heal">the ammount of healing to be applied</param>
    public void Heal(float heal)
    {
        hp = Mathf.Min(maxHp, hp + heal);
    }

    /// <summary>
    /// Will apply a movement speed change based on ms.
    /// ms is a percentage change to the current movement speed.
    /// To buff the movement speed by 10% do <code>ms=1.1f</code>.
    /// To debuff the movement speed by 10% do <code>ms=0.9f</code>.
    /// </summary>
    /// <param name="ms">the percentage change to be multiplied into movement speed for this entity</param>
    public void MultiplyMovementSpeed(float ms)
    {
        Debug.Assert(ms > 0f, "ms must be a positive float. U@se 0.5 to half the movement speed instead.");
        movement_speed *= ms;
    }

    public void AddAttackPhysical(float atk)
    {
        attack_physical += atk;
    }

    public void MultiplyAttackPhysical(float atkMul)
    {
        attack_physical *= atkMul;
    }

    public void AddAttackEmotional(float atk)
    {
        attack_emotional += atk;
    }

    public void MultiplyAttackEmotional(float atkMul)
    {
        attack_emotional *= atkMul;
    }

    public void AddArmourPhysical(float armour)
    {
        armour_physical += armour;
    }

    public void MultiplyArmourPhysical(float armourMul)
    {
        armour_physical *= armourMul;
    }

    public void AddArmourEmotional(float armour)
    {
        armour_emotional += armour;
    }

    public void MultiplyArmourEmotional(float armourMul)
    {
        armour_emotional *= armourMul;
    }


    /// <summary>
    /// Moves the entity towards dir.
    /// dir will only provide direction, the amount the entity moves will be determined by its movement speed.
    /// the direction should be -1, 0 or +1 for x and y.
    /// </summary>
    /// <param name="dir">A vector2 for the direction, where both x and y can be one of -1, 0 or +1.</param>
    public void MoveEntityOnDirection(Vector2 dir)
    {
        rb.MovePosition(rb.position + (dir.normalized * movement_speed * Time.fixedDeltaTime));
    }
}
