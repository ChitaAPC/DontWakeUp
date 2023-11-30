using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameAudioController : MonoBehaviour
{
    [SerializeField] bool isBad;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.StopAllMusic();
        if (isBad)
            AudioManager.instance.PlaySong("BadEnd");
        else
            AudioManager.instance.PlaySong("GoodEnd");
    }

}
