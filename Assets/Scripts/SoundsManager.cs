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
    bool activeSignalSound;
    bool activeRandomSound;
    int r;
    
    public void Start()
    {
        motivationMessage.text = "";
        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;
        activeSignalSound = true;
        activeRandomSound = true;
    }

    public void AddSound()
    {
        activeSignalSound = true;
        signalAudioSource.mute = false;

        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;
        motivationAudioSource.mute = false;
        activeRandomSound = true;
    }

    public void PlaySignalSound()
    {
        if(activeSignalSound)
        {
            signalAudioSource.PlayOneShot(signalAudioSource.clip);
            motivationMessage.text = "Toma el aire";
            activeSignalSound = false;
        }
    }

    public void PlayRandomSound()
    {
        if(activeRandomSound)
        {
            motivationAudioSource.PlayOneShot(motivationAudioSource.clip);
            motivationMessage.text = motivationSounds[r].text;
            activeRandomSound = false;
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

    public void StopRandomSound()
    {
        if (!motivationAudioSource.isPlaying && !activeRandomSound)
        {
            motivationAudioSource.Stop();
            motivationAudioSource.mute = true;
        }        
    }
}
