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
    bool active = true;

    
    public void InitializeMotivationSound()
    {
        motivationMessage.text = "";
    }

    public void AddSignalSound()
    {
        active = true;
        signalAudioSource.mute = false;
    }

    public void PlaySignalSound()
    {
        if(active)
        {
            signalAudioSource.PlayOneShot(signalAudioSource.clip);
            active = false;
        }
    }

    public void PlayRandomSound()
    {
        int r = Random.Range(0, motivationSounds.Count);
        if(motivationAudioSource.clip == null)
        {
            motivationMessage.text = motivationSounds[r].text;
            motivationAudioSource.clip = motivationSounds[r].clip;
            //motivationAudioSource.Play();
            motivationAudioSource.PlayOneShot(motivationAudioSource.clip);
        }
    }

    public void StopSignalSound()
    {
        if (!signalAudioSource.isPlaying && !active)
        {
            signalAudioSource.Stop();
            signalAudioSource.mute = true;
        }
    }

    public void StopRandomSound()
    {
        if (!motivationAudioSource.isPlaying)
        {
            motivationAudioSource.Stop();
            motivationAudioSource.clip =  null;
            motivationMessage.text = "";
        }        
    }
}
