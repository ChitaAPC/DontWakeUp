using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    [SerializeField]
    private GameObject BattleUiCanvas;

    private void Start()
    {
        EventHandler.instance.BattleStartEvent.AddListener(OnBattleStart);
    }

    private void OnBattleStart(AbstractEntityController player, AbstractEntityController enemy)
    {
        Time.timeScale = 0f;
        BattleUiCanvas.SetActive(true);
    }

}
