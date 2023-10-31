using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationManager : NetworkBehaviour
{
    public Animator anim;
    public NetworkVariable<bool> correndo = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> sonolento = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> flutuando = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> dormindo = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hit = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> interagindo = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
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
        if (IsClient)
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
        anim.SetBool("correndo", current);
    }
    void ChangeAnimatorSonolento(bool previous, bool current)
    {
        anim.SetBool("sonolento", current);
    }
    void ChangeAnimatorFlutuando(bool previous, bool current)
    {
        anim.SetBool("flutuando", current);
    }
    void ChangeAnimatorDormindo(bool previous, bool current)
    {
        anim.SetBool("dormindo", current);
    }
    void ChangeAnimatorHit(bool previous, bool current)
    {
        anim.SetBool("hit", current);
    }
    void ChangeAnimatorInteragindo(bool previous, bool current)
    {
        anim.SetBool("interagindo", current);
    }

    public void Adormecer()
    {
        anim.SetBool("dormindoLoop", true);
    }

    public void Acordar()
    {
        anim.SetBool("dormindoLoop", false);
    }
}
