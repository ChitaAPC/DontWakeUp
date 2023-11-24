using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbstractEntityController;

public class ScaleManager : MonoBehaviour
{

    private Vector2 offsets = new Vector2(16,-10);

    private Vector2 startingCoord = new Vector2(16, -16);

    private List<int> buff_counts = new List<int>();

    [SerializeField] private GameObject Buff_prefab;
    [SerializeField] private GameObject Debuff_prefab;

    private void Awake()
    {
        buff_counts.Add(0);
    }



    private void OnBuffApplied(EntityStats modifier)
    {

        //todo figureout which buff/debuff it is

        int row = Random.Range(0, buff_counts.Count);
        if (row == buff_counts.Count)
        {
            buff_counts.Add(0);
        }

        GameObject buff = Instantiate(Buff_prefab);
        RectTransform buff_rect = buff.GetComponent<RectTransform>();
        float x_offset;
        if (row % 2 == 0)
        {
            //even row
            
        }

    }

}
