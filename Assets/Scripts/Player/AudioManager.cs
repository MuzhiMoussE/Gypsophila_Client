using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class AudioManager : SingletonMonoBase<AudioManager>
{
    public float fxVolume = 1f;
    [SerializeField]private AudioClip Moving;
    [SerializeField] private AudioClip Jumping;
    [SerializeField] private AudioClip ToSummon;
    [SerializeField] private AudioClip Releasing;
    [SerializeField] private AudioClip Recording;
    [SerializeField] private AudioClip Dragging;
    [SerializeField] private AudioClip Die;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeAudio(Global.PlayerState state)
    {
        if(state == Global.PlayerState.Idle)
        {
            audioSource.pitch = 1f;
            audioSource.Pause();
        }
        else if(state == Global.PlayerState.Moving)
        {
            audioSource.clip = Moving;
            audioSource.loop = true;
            audioSource.pitch = 1f;
            audioSource.volume = fxVolume;
            audioSource.Play();
        }
        else if( state == Global.PlayerState.Jumping)
        {
            audioSource.clip = Jumping;
            audioSource.loop = false;
            audioSource.pitch = 1f;
            audioSource.volume = fxVolume;
            audioSource.Play();
        }
        else if(state == Global.PlayerState.Dragging)
        {
            audioSource.clip = Dragging;
            audioSource.loop = true;
            audioSource.pitch = 1.67f;
            audioSource.volume = fxVolume;
            audioSource.Play();
        }
        else if(state == Global.PlayerState.Die)
        {
            audioSource.clip = Die;
            audioSource.loop = false;
            audioSource.pitch = 1f;
            audioSource.volume = fxVolume;
            audioSource.Play();
        }
    }


}
