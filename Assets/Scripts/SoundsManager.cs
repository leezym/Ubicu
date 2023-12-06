using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoundsManager : MonoBehaviour
{
    [Header("ATTACHED")]
    public List<MotivationSound> motivationSounds;
    public AudioSource motivationAudioSource;
    public AudioSource signalAudioSource;
    public TMP_Text motivationMessage;
    public AudioClip signalAudioClip;
    public bool activeSignalSound;
    bool activeMotivationSound;
    int r;
    
    public void InitializeSounds()
    {
        motivationMessage.text = "";
        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;
        activeSignalSound = true;

        StartCoroutine(PlaySignalSound());
    }

    public void AddSound()
    {
        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;
    }

    public IEnumerator PlaySignalSound()
    { 
        if(!motivationAudioSource.isPlaying && activeSignalSound)
        {
            activeSignalSound = false;

            yield return new WaitForSeconds(1.5f);      

            signalAudioSource.PlayOneShot(signalAudioSource.clip);
            signalAudioSource.mute = false;
            motivationMessage.text = "Toma el aire";
            
            activeMotivationSound = true;
        }
    }

    public IEnumerator PlayMotivationSound()
    {
        if(!signalAudioSource.isPlaying && activeMotivationSound)
        {
            activeMotivationSound = false;

            yield return new WaitForSeconds(1f);

            motivationAudioSource.PlayOneShot(motivationAudioSource.clip);
            motivationAudioSource.mute = false;
            motivationMessage.text = motivationSounds[r].text;
        }
    }

    public void StopSignalSound()
    {
        if (!signalAudioSource.isPlaying && !activeSignalSound)
        {
            signalAudioSource.Stop();
            signalAudioSource.mute = true;
        }
    }

    public void StopMotivationSound()
    {
        if (!motivationAudioSource.isPlaying && !activeMotivationSound)
        {
            motivationAudioSource.Stop();
            motivationAudioSource.mute = true;
        }        
    }
}