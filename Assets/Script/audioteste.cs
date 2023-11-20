using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class audioteste : Singleton<audioteste>
{
   private AudioSource _audioSource;

    [SerializeField] private AudioClip Nowaudioclip;
    public override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }
    public void playnomaudioclip()
    {
        _audioSource.clip = Nowaudioclip;
        _audioSource.Play();
    }
}
