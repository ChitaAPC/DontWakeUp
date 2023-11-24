using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private GameObject tooltipObject;
    private string myTooltip;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "DescriptionTooltip")
                tooltipObject = transform.GetChild(i).gameObject;
        }
        tooltipObject.GetComponentInChildren<TextMeshProUGUI>().text = myTooltip;
    }

    public void SetTooltipText(string text)
    {
        myTooltip = text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipObject.SetActive(true);
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipObject.SetActive(false);
    }
}
