using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;


public class audioevenbtes : NetworkBehaviour
{
    [SerializeField] private AudioSource somDeAtaque;
    public NetworkVariable<bool> atacando;
   

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            somDeAtaque.GetComponent<AudioSource>();
            atacando.OnValueChanged += ChangersomAtacando;
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsClient && IsOwner)
        {
            atacando.OnValueChanged -= ChangersomAtacando;
        }
    }
   
    void ChangersomAtacando(bool previous ,bool current )
    {
        somDeAtaque.Play();
        Debug.Log("tocou o audio");
    }
   
}
