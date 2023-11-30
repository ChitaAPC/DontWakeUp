using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAudioHandler : MonoBehaviour
{

    private void Awake()
    {
        EventHandler.instance.BattleStartEvent.AddListener(OnBattleStart);
        EventHandler.instance.BattleEndEvent.AddListener(OnBattleEnd);
    }

    private void Start()
    {
        if (AudioManager.instance == null)
        {
            Debug.LogError("Audio Manager not initialised properly, no audio will be played");
            return;
        }

        AudioManager.instance.PlayTwoSongsInSync("WorldSong", "CombatSong");

        //AudioManager.instance.ChangeSongs("WorldSong", false);
    }

    private void OnBattleStart(AbstractEntityController player, AbstractEntityController enemy)
    {
        //todo: replace that with ChangeSongs to play the correct song
        if (enemy.tag == "Boss")
        {
            AudioManager.instance.StopAllMusic();
            AudioManager.instance.PlaySong("BossIntro");
            AudioManager.instance.PlaySongWithDelay("BossLoop", AudioManager.instance.GetSongLength("BossIntro"));
        }
        else
        {
            AudioManager.instance.SetWantedFadeInSong("CombatSong");
        }
    }

    private void OnBattleEnd(AbstractEntityController player, AbstractEntityController enemy)
    {
        AudioManager.instance.SetWantedFadeInSong("WorldSong");
    }





}
