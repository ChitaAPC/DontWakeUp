using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using static AbstractEntityController;

public class ScaleManager : MonoBehaviour
{

    private Vector2 offsets = new Vector2(16,-10);

    private Vector2 startingCoord = new Vector2(16, -16);

    private List<int> scaleCounts = new List<int>();

    [SerializeField] private GameObject buffPrefab;
    [SerializeField] private GameObject debuffPrefab;

    [SerializeField] Color MaxHpColour;
    [SerializeField] Color MovementSpeedColour;
    [SerializeField] Color CombatSpeedColour;
    [SerializeField] Color PhysicalAttackColour;
    [SerializeField] Color EmotionalAttackColour;
    [SerializeField] Color PhysicalDefenceColour;
    [SerializeField] Color EmotionalDefenceColour;

    private void Awake()
    {
        scaleCounts.Add(0);
        EventHandler.instance.BuffAppliedEvent.AddListener(OnBuffApplied);
    }

    private void OnBuffApplied(EntityStats modifier)
    {

        //todo figureout which buff it is
        GameObject buff = Instantiate(buffPrefab,transform);
        buff.GetComponent<Image>().color = GetBuffColour(modifier);
        buff.GetComponent<ScaleController>().SetTooltipText(GetBuffTooltipText(modifier));
        PlaceScale(buff);

        //todo figure out what debuff it is
        GameObject debuff = Instantiate(debuffPrefab, transform);
        debuff.GetComponent<Image>().color = GetDebuffColour(modifier);
        debuff.GetComponent<ScaleController>().SetTooltipText(GetDebuffTooltipText(modifier));
        PlaceScale(debuff);
    }

    private string GetBuffTooltipText(EntityStats modifier)
    {
        if (modifier.maxHp > 0)
            return "Buff\nMax HP";
        if (modifier.movement_speed > 0)
            return "Buff\nMovement Speed";
        if (modifier.combat_speed > 0)
            return "Buff\nCombat Speed";
        if (modifier.attack_physical > 0)
            return "Buff\nPhysical Attack";
        if (modifier.attack_emotional > 0)
            return "Buff\nEmotional Attack";
        if (modifier.armour_physical > 0)
            return "Buff\nPhysical Armour";
        if (modifier.armour_emotional > 0)
            return "Buff\nEmotional Armour";
        return "Buff";
    }

    private string GetDebuffTooltipText(EntityStats modifier)
    {
        if (modifier.maxHp < 0)
            return "Debuff\nMax HP";
        if (modifier.movement_speed < 0)
            return "Debuff\nMovement Speed";
        if (modifier.combat_speed < 0)
            return "Debuff\nCombat Speed";
        if (modifier.attack_physical < 0)
            return "Debuff\nPhysical Attack";
        if (modifier.attack_emotional < 0)
            return "Debuff\nEmotional Attack";
        if (modifier.armour_physical < 0)
            return "Debuff\nPhysical Armour";
        if (modifier.armour_emotional < 0)
            return "Debuff\nEmotional Armour";
        return "Debuff";
    }

    private Color GetBuffColour(EntityStats modifier)
    {
        if (modifier.maxHp > 0)
            return MaxHpColour;
        if (modifier.movement_speed > 0)
            return MovementSpeedColour;
        if (modifier.combat_speed > 0)
            return CombatSpeedColour;
        if (modifier.attack_physical > 0)
            return PhysicalAttackColour;
        if (modifier.attack_emotional > 0)
            return EmotionalAttackColour;
        if (modifier.armour_physical > 0)
            return PhysicalDefenceColour;
        if (modifier.armour_emotional > 0)
            return EmotionalDefenceColour;
        return Color.white;
    }

    private Color GetDebuffColour(EntityStats modifier)
    {
        if (modifier.maxHp < 0)
            return MaxHpColour;
        if (modifier.movement_speed < 0)
            return MovementSpeedColour;
        if (modifier.combat_speed < 0)
            return CombatSpeedColour;
        if (modifier.attack_physical < 0)
            return PhysicalAttackColour;
        if (modifier.attack_emotional < 0)
            return EmotionalAttackColour;
        if (modifier.armour_physical < 0)
            return PhysicalDefenceColour;
        if (modifier.armour_emotional < 0)
            return EmotionalDefenceColour;
        return Color.white;
    }

    private void PlaceScale(GameObject scaleToPlace)
    {
        RectTransform scaleRect = scaleToPlace.GetComponent<RectTransform>();

        int row;
        if (scaleCounts[0] == 0)
            row = 0;
        else
            row = Random.Range(0, scaleCounts.Count + 1);

        if (row == scaleCounts.Count)
        {
            scaleCounts.Add(0);
        }

        float x;
        if (row % 2 == 0)
        {
            //even row
            x = (startingCoord.x / 2f) + (offsets.x * scaleCounts[row]);
        }
        else
        {
            x = startingCoord.x + (offsets.x * scaleCounts[row]);
        }

        float y = startingCoord.y + (offsets.y * row);
        scaleRect.anchoredPosition = new Vector3(x, y);
        scaleCounts[row]++;
    }

}
