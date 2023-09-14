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
    bool activeMotivationSound;
    bool activeSignalSound;
    int r;
    
    public void InitializeSounds()
    {
        //Sonidos de Motivacion
        activeMotivationSound = false;
        motivationAudioSource.mute = true;
        motivationMessage.text = "";
        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;

        // Señal de Toma el aire
        activeSignalSound = true;
        signalAudioSource.mute = false;
    }

    public void AddSound()
    {
        activeSignalSound = true;
        signalAudioSource.mute = false;

        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;
    }

    public void PlaySignalSound()
    {
        if(activeSignalSound)
        {
            signalAudioSource.PlayOneShot(signalAudioSource.clip);
            motivationMessage.text = "Toma el aire";
            activeSignalSound = false;           

            activeMotivationSound = true;
            motivationAudioSource.mute = false;            
        }
    }

    public void PlayRandomSound()
    {
        if(activeMotivationSound && signalAudioSource.mute)
        {
            motivationAudioSource.PlayOneShot(motivationAudioSource.clip);
            motivationMessage.text = motivationSounds[r].text;
            activeMotivationSound = false;
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
        if (!motivationAudioSource.isPlaying && !activeMotivationSound)
        {
            motivationAudioSource.Stop();
            motivationAudioSource.mute = true;
        }        
    }
}