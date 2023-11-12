using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{

    [SerializeField]
    private GameObject BattleUiCanvas;

    [SerializeField]
    private TextMeshProUGUI battleTextDisplay;

    [SerializeField]
    private List<Button> buttons;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Slider enemyHealthBar;
    [SerializeField]
    private Slider playerHealthBar;


    private bool waitingForPlayerTurn;

    private string battleString;

    private bool wantsAttack;
    private bool wantsPhysical;

    private bool waitingForNextDesc;


    private void Start()
    {
        EventHandler.instance.BattleStartEvent.AddListener(OnBattleStart);
        
    }

    private void OnBattleStart(AbstractEntityController player, AbstractEntityController enemy)
    {
        Time.timeScale = 0f;
        BattleUiCanvas.SetActive(true);
        DisablePlayerTurn();
        battleString = $"Do battle with {enemy.gameObject.name}";
        waitingForNextDesc = false;
        nextButton.interactable = false;

        enemyHealthBar.maxValue = enemy.maxHp;
        enemyHealthBar.value = enemy.hp;
        playerHealthBar.maxValue = player.maxHp;
        playerHealthBar.value = player.hp;

        StartCoroutine(BattleCo(player, enemy));
    }

    //display text
    //wait for player action
    //decide action order
    //do first action
    //display text for first action
    //do second action
    //display text for second action


    private IEnumerator BattleCo(AbstractEntityController player, AbstractEntityController enemy)
    {
        while (player.hp > 0 && enemy.hp > 0)
        {
            DisplayBattleText();
            EnablePlayerTurn();
            while (waitingForPlayerTurn)
            {
                yield return null;
            }
            DoFirstAction(player, enemy);
            while (waitingForNextDesc)
            {
                yield return null;
            }
            if (player.hp > 0 && enemy.hp > 0)
            {
                //only do second action if both are still alive
                DoSecondAction(player, enemy);
                while (waitingForNextDesc)
                {
                    yield return null;
                }
            }
            HandleEndOfTurnString(player, enemy);
        }
        DisplayBattleText();
        SetWaitingForNextBtn();
        while (waitingForNextDesc)
        {
            yield return null;
        }
        HandleEndOfCombat(player, enemy);
    }

    private void HandleEndOfCombat(AbstractEntityController player, AbstractEntityController enemy)
    {
        //todo!
        if (player.hp > 0)
        {
            OnPlayerLivesEndOfCombat(player, enemy);
        }
        else
        {
            //todo: load into game over scene
        }
    }

    private void OnPlayerLivesEndOfCombat(AbstractEntityController player, AbstractEntityController enemy)
    {
        if (enemy.hp <= 0f)
        {
            player.ApplyBuffsModifiers(enemy.buffs);
            Destroy(enemy.gameObject);
            BattleUiCanvas.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            //todo? are we making it possible for a fight to ent without killing the enemy?
        }
    }

    private void DoFirstAction(AbstractEntityController player, AbstractEntityController enemy)
    {
        if (enemy.combat_speed > player.combat_speed)
        {
            battleString = enemy.DoAIAction(player);
        }
        else
        {
            battleString = HandlePlayerMove(player, enemy);
        }

        enemyHealthBar.value = enemy.hp;
        playerHealthBar.value = player.hp;

        DisplayBattleText();
        SetWaitingForNextBtn();

    }

    private void DoSecondAction(AbstractEntityController player, AbstractEntityController enemy)
    {
        if (enemy.combat_speed <= player.combat_speed)
        {
            battleString = enemy.DoAIAction(player);
        }
        else
        {
            battleString = HandlePlayerMove(player, enemy);
        }

        enemyHealthBar.value = enemy.hp;
        playerHealthBar.value = player.hp;

        DisplayBattleText();
        SetWaitingForNextBtn();
    }

    private void SetWaitingForNextBtn()
    {
        waitingForNextDesc = true;
        nextButton.interactable = true;
    }


    private void DisplayBattleText()
    {
        battleTextDisplay.text = battleString;
    }

    private void EnablePlayerTurn()
    {
        foreach (Button b in buttons)
        {
            b.interactable = true;
        }
        waitingForPlayerTurn = true;
    }

    private void DisablePlayerTurn()
    {
        foreach (Button b in buttons)
        {
            b.interactable = false;
        }
        waitingForPlayerTurn = false;
    }

    private void DoPlayerMove(bool attack, bool physical)
    {
        wantsAttack = attack;
        wantsPhysical = physical;
        DisablePlayerTurn();
    }

    public void OnPlayerPhysicalAttackBtn()
    {
        DoPlayerMove(true, true);
    }

    public void OnPlayerMentalAttackBtn()
    {
        DoPlayerMove(true, false);
    }

    public void OnPlayerPhysicalDefendBtn()
    {
        DoPlayerMove(false, true);
    }

    public void OnPlayerMentalDefendBtn()
    {
        DoPlayerMove(false, false);
    }

    private string HandlePlayerMove(AbstractEntityController player, AbstractEntityController enemy)
    {
        string description = "";

        if (wantsAttack)
        {
            if (wantsPhysical)
            {
                enemy.TakeDamage(player.attack_physical, true);
                description = $"You just attacked {enemy.gameObject.name} for {Mathf.RoundToInt(player.attack_physical)} damage";
            }
            else
            {
                enemy.TakeDamage(player.attack_emotional, false);
                description = $"You just insulted {enemy.gameObject.name} for {Mathf.RoundToInt(player.attack_emotional)} damage";
            }
        }

        return description;
    }

    public void OnNextBtn()
    {
        waitingForNextDesc = false;
        nextButton.interactable = false;
    }

    private void HandleEndOfTurnString(AbstractEntityController player, AbstractEntityController enemy)
    {
        if (player.hp <= 0)
        {
            battleString = "You Lost!";
            //todo handle game over scene
        }
        else if (enemy.hp <= 0)
        {
            battleString = $"You have defeated {enemy.gameObject.name}";
            //todo apply buff/debuff
        }
        else
        {
            battleString = "What will you do next?";
        }
    }

}
