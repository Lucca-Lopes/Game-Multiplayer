using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationManager : NetworkBehaviour
{
    public Animator anim;
    public NetworkVariable<bool> correndo;
    public NetworkVariable<bool> sonolento;
    public NetworkVariable<bool> flutuando;
    public NetworkVariable<bool> dormindo;
    public NetworkVariable<bool> hit;
    public NetworkVariable<bool> interagindo;

    public override void OnNetworkSpawn()
    {
        anim = GetComponent<Animator>();
        if (IsClient && IsOwner)
        {
            correndo.OnValueChanged += ChangeAnimatorCorrendo;
            sonolento.OnValueChanged += ChangeAnimatorSonolento;
            flutuando.OnValueChanged += ChangeAnimatorFlutuando;
            dormindo.OnValueChanged += ChangeAnimatorDormindo;
            hit.OnValueChanged += ChangeAnimatorHit;
            interagindo.OnValueChanged += ChangeAnimatorInteragindo;
        }
    }

    public override void OnNetworkDespawn()
    {
        anim = GetComponent<Animator>();
        if (IsClient && IsOwner)
        {
            correndo.OnValueChanged -= ChangeAnimatorCorrendo;
            sonolento.OnValueChanged -= ChangeAnimatorSonolento;
            flutuando.OnValueChanged -= ChangeAnimatorFlutuando;
            dormindo.OnValueChanged -= ChangeAnimatorDormindo;
            hit.OnValueChanged -= ChangeAnimatorHit;
            interagindo.OnValueChanged -= ChangeAnimatorInteragindo;
        }
    }

    void ChangeAnimatorCorrendo(bool previous, bool current)
    {
        anim.SetBool("Correndo", current);
    }
    void ChangeAnimatorSonolento(bool previous, bool current)
    {
        anim.SetBool("Sonolento", current);
    }
    void ChangeAnimatorFlutuando(bool previous, bool current)
    {
        anim.SetBool("Flutuando", current);
    }
    void ChangeAnimatorDormindo(bool previous, bool current)
    {
        anim.SetBool("Dormindo", current);
    }
    void ChangeAnimatorHit(bool previous, bool current)
    {
        anim.SetBool("Hit", current);
    }
    void ChangeAnimatorInteragindo(bool previous, bool current)
    {
        anim.SetBool("Interagindo", current);
    }

    public void Adormecer()
    {
        anim.SetBool("DormindoLoop", true);
    }

    public void Acordar()
    {
        anim.SetBool("DormindoLoop", false);
    }
}
