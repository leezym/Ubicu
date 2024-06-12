using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance {get; private set;}

    [Header("ATTACHED")]
    public AudioClip signalAudioClip;
    public List<MotivationSound> motivationSounds;
    public AudioSource motivationAudioSource;
    public AudioSource signalAudioSource;
    public TMP_Text motivationMessage;

    [Header("IN GAME")]
    bool activeMotivationSound;
    public bool activeSignalSound;
    int r;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        DisableAudioReverbFilter(motivationAudioSource);
        DisableAudioReverbFilter(signalAudioSource);
    }
    
    public void InitializeSounds()
    {
        motivationMessage.text = "";
        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;
        
        signalAudioSource.clip = signalAudioClip;
        activeSignalSound = true;

        // Set audio sources to stream for longer clips
        motivationAudioSource.clip.LoadAudioData();
        signalAudioSource.clip.LoadAudioData();

        StartCoroutine(PlaySignalSound());
    }

    public void AddSound()
    {
        r = Random.Range(0, motivationSounds.Count);
        motivationAudioSource.clip = motivationSounds[r].clip;

        // Load audio data for the new clip
        motivationAudioSource.clip.LoadAudioData();
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

    private void DisableAudioReverbFilter(AudioSource audioSource)
    {
        var reverbFilter = audioSource.GetComponent<AudioReverbFilter>();
        if (reverbFilter != null)
        {
            reverbFilter.enabled = false;
        }
    }
}