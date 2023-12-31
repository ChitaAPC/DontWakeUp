using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;

    private float timeStart = 0f;
    private float minutesPerLevel = 15f;

    // Start is called before the first frame update
    void Start()
    {
        int hours = Mathf.FloorToInt((timeStart + PlayerController.PlayerLevel * minutesPerLevel) / 60f);
        int minuts = Mathf.FloorToInt((timeStart + PlayerController.PlayerLevel * minutesPerLevel) % 60f);
        Time.timeScale = 1f;
        scoreText.text = $"{hours:D2}:{minuts:D2}";
    }

}
