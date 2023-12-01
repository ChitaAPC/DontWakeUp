using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class AbstractEntityController : MonoBehaviour
{

    protected GameObject player = null;
    protected float detectionRadious;

    public struct EntityStats
    {
        public float hp;
        public float maxHp;

        public float movement_speed;    //debuff
        public float combat_speed;      //debuff

        public float attack_physical;   //buff
        public float attack_emotional;  //buff

        public float armour_physical;   //buff debuff
        public float armour_emotional;  //buff debuff
    }


    public float hp { private set; get; }
    public float maxHp { private set; get; }

    public float movement_speed { private set; get; }
    public float combat_speed { private set; get; }

    public float attack_physical { private set; get; }
    public float attack_emotional { private set; get; }

    public float armour_physical { private set; get; }
    public float armour_emotional { private set; get; }

    public EntityStats buffs { private set; get; }

    
    private Rigidbody2D rb;

    protected Animator animator;

    private bool innitialised;

    //public int ID { private set;  get; }

    //private static int ID_COUNT;

    public bool is_def_physical { protected set; get; }
    public bool is_def_emotional { protected set; get; }

    private void Awake()
    {
        innitialised = false;
        is_def_physical = false;
        is_def_emotional = false;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        OnAwake();
        Debug.Assert(innitialised, "ERROR: you must call \"InnitialiseProperties\" during OnAwake to initialise all of the values for this entity");
        //ID = ID_COUNT;
        //ID_COUNT++;
    }

    protected abstract void OnAwake();

    protected void InnitialiseProperties(EntityStats stats, EntityStats buffs)
    {
        hp = stats.maxHp;
        maxHp = stats.maxHp;
        movement_speed = stats.movement_speed;
        combat_speed = stats.combat_speed;
        attack_physical = stats.attack_physical;
        attack_emotional = stats.attack_emotional;
        armour_physical = stats.armour_physical;
        armour_emotional = stats.armour_emotional;
        this.buffs = buffs;

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
    /// <param name="isPhysical">A flag that dictates if the incoming damage is physical</param>
    /// <returns>True if the new HP is bellow or equal to zero, False otherwise.</returns>
    public bool TakeDamage(float dmg, bool isPhysical)
    {
        float damageToTake;
        if (isPhysical)
        {
            if (is_def_physical)
            {
                damageToTake = dmg - (2 * armour_physical); //overarmour can heal
            }
            else
            {
                damageToTake = Mathf.Max(0f, dmg - armour_physical);
            }
        }
        else 
        {
            if (is_def_emotional)
            {
                damageToTake = dmg - (2 * armour_emotional); //overarmour can heal
            }
            else
            {
                damageToTake = Mathf.Max(0f, dmg - armour_emotional);
            }
        }
        hp = Mathf.Min(Mathf.Max(0f, hp - damageToTake), maxHp);
        if (hp > 0f)
        {
            if (damageToTake > 0)
            {
                animator.SetTrigger("Hurt");
            }
            else
            {
                //todo heal animation?
            }
        }
        else
        {
            animator.SetTrigger("Dead");
        }

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
    public void MoveEntityOnDirection(Vector2 dir, bool normalised = true)
    {

        Vector2 wantedDir;

        if (normalised)
        {
            wantedDir = dir.normalized;
        }
        else
        {
            wantedDir = dir;
        }
        rb.MovePosition(rb.position + (wantedDir * movement_speed * Time.fixedDeltaTime));
    }

    /// <summary>
    /// A function that handles the battle action for the entity when it is controlled by the AI and returns a text description
    /// </summary>
    /// <param name="player">The player controller script to be used</param>
    /// <returns>a text description of the action taken</returns>
    public abstract string DoAIAction(AbstractEntityController player);


    /// <summary>
    /// Applies the specified buffs into the entity in a value safe manner
    /// </summary>
    /// <param name="buffs">the buffs to be applied as addatives</param>
    public void ApplyBuffsModifiers(EntityStats buffs)
    {
        maxHp += buffs.maxHp;
        hp = Mathf.Min(maxHp, hp + buffs.hp);
        movement_speed += buffs.movement_speed;
        combat_speed += buffs.combat_speed;
        attack_physical += buffs.attack_physical;
        attack_emotional += buffs.attack_emotional;
        armour_physical = Mathf.Max(0f, armour_physical + buffs.armour_physical);
        armour_emotional = Mathf.Max(0f, armour_emotional + buffs.armour_emotional);

        EventHandler.instance.BuffAppliedEvent.Invoke(buffs);
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void SetDefenceMode(bool mode, bool physical)
    {
        if (physical)
        {
            is_def_physical = mode;
        }
        else
        {
            is_def_emotional = mode;
        }
    }


    public void SetPlayerTarget(GameObject player)
    {
        this.player = player;
    }

    protected bool IsIdleMode()
    {
        if (player == null)
        {
            return true;
        }

        return Vector2.Distance(transform.position, player.transform.position) >= detectionRadious;
    }


    protected void LevelUp()
    {
        maxHp *= 1.1f;
        hp = maxHp;

        attack_physical *= 1.01f;
        attack_emotional *= 1.01f;

        armour_physical *= 1.01f;
        armour_emotional *= 1.01f;

        movement_speed *= 1.01f;
        combat_speed *= 1.01f;

    }

    public virtual void OnBattleStartLogic()
    {
        //do nothing by default
    }
}
