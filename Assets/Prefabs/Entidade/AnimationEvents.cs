using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class AnimationEvents : NetworkBehaviour
{
    public Animator anim;
    public NetworkVariable<bool> andando;
    public NetworkVariable<bool> levantandoCrianca;
    public NetworkVariable<bool> soltandoCrianca;
    public NetworkVariable<bool> usandoHabilidade;
    public NetworkVariable<bool> atacando;

    public VisualEffect direito;
    public VisualEffect esquerdo;
    public VisualEffect particulaAtaque;
    public void PlayVisualEffects(int pe) 
    {
        if(pe == 1)
        {
            esquerdo.Play();
        }
        else if(pe == 2)
        {
            direito.Play();
        }
    }

    public void AttackParticles()
    {
        particulaAtaque.Play();
    }

    public override void OnNetworkSpawn()
    {
        anim = GetComponent<Animator>();
        if (IsClient && IsOwner)
        {
            andando.OnValueChanged += ChangeAnimatorAndando;
            levantandoCrianca.OnValueChanged += ChangeAnimatorLevantandoCrianca;
            soltandoCrianca.OnValueChanged += ChangeAnimatorSoltandoCrianca;
            usandoHabilidade.OnValueChanged += ChangeAnimatorUsandoHabilidade;
            atacando.OnValueChanged += ChangeAnimatorAtacando;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient && IsOwner)
        {
            andando.OnValueChanged -= ChangeAnimatorAndando;
            levantandoCrianca.OnValueChanged -= ChangeAnimatorLevantandoCrianca;
            soltandoCrianca.OnValueChanged -= ChangeAnimatorSoltandoCrianca;
            usandoHabilidade.OnValueChanged -= ChangeAnimatorUsandoHabilidade;
            atacando.OnValueChanged -= ChangeAnimatorAtacando;
        }
    }

    void ChangeAnimatorAndando(bool previous, bool current)
    {
        anim.SetBool("andando", current);
    }
    void ChangeAnimatorLevantandoCrianca(bool previous, bool current)
    {
        anim.SetBool("levantandoCrianca", current);
    }
    void ChangeAnimatorSoltandoCrianca(bool previous, bool current)
    {
        anim.SetBool("soltandoCrianca", current);
    }
    void ChangeAnimatorUsandoHabilidade(bool previous, bool current)
    {
        anim.SetBool("usandoHabilidade", current);
    }
    void ChangeAnimatorAtacando(bool previous, bool current)
    {
        anim.SetBool("atacando", current);
    }
}
