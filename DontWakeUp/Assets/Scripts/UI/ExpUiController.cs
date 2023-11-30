using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpUiController : MonoBehaviour
{

    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    // Update is called once per frame
    void Update()
    {
        int level = Mathf.FloorToInt(PlayerController.PlayerLevel);
        float expLevel = PlayerController.PlayerLevel - level;

        expSlider.value = expLevel;
        levelText.text = level.ToString();
    }
}
