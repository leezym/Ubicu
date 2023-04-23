using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoundsManager : MonoBehaviour
{
    [Header("ATTACHED")]
    public List<MotivationSound> motivationSounds;
    public AudioSource audioSource;
    public TMP_Text motivationMessage;

    [Header("IN GAME")]
    public float tempSeconds;

    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void PlayRandomSound(){
        int r = Random.Range(0, motivationSounds.Count);
        audioSource.clip = motivationSounds[r].clip;
        audioSource.Play();
        motivationMessage.text = motivationSounds[r].text;
    }

    void PlayRandomTime(){
        List<int> seconds = new List<int>{20,30,60};
        int r = Random.Range(0, seconds.Count);
        tempSeconds += seconds[r];
    }
}
