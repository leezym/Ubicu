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

    
    public void InitializeMotivationSound()
    {
        tempSeconds = PlayRandomTime();
        motivationMessage.text = "";
    }

    public void PlayRandomSound(){
        int r = Random.Range(0, motivationSounds.Count);
        if(audioSource.clip == null)
        {
            motivationMessage.text = motivationSounds[r].text;
            audioSource.clip = motivationSounds[r].clip;
            //audioSource.Play();
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    public void StopRandomSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip =  null;
            motivationMessage.text = "";
        }        
    }

    public float PlayRandomTime()
    {
        List<int> seconds = new List<int>{10,20,40};
        int r = Random.Range(0, seconds.Count);
        return seconds[r];
    }
}
