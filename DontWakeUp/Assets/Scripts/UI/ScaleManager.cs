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

    private void Awake()
    {
        scaleCounts.Add(0);
        EventHandler.instance.BuffAppliedEvent.AddListener(OnBuffApplied);
    }

    private void OnBuffApplied(EntityStats modifier)
    {

        //todo figureout which buff it is
        GameObject buff = Instantiate(buffPrefab,transform);
        Image buffImage = buff.GetComponent<Image>();
        buffImage.color = new Color(Random.value, Random.value, Random.value);
        PlaceScale(buff);

        //todo figure out what debuff it is
        GameObject debuff = Instantiate(debuffPrefab, transform);
        Image debuffImage = debuff.GetComponent<Image>();
        debuffImage.color = new Color(Random.value, Random.value, Random.value);
        PlaceScale(debuff);
    }

    //private Color GetBuffColour(EntityStats modifier)
    //{

    //}

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
