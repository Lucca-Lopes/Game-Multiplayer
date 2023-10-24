using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AnimationEvents : MonoBehaviour
{
    public Animator anim;
    public bool andando;
    public bool levantandoCrianca;
    public bool soltandoCrianca;
    public bool usandoHabilidade;
    public bool atacando;

    /*public VisualEffect direito;
    public VisualEffect esquerdo;
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
    }*/

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        anim.SetBool("andando", andando);
        anim.SetBool("levantandoCrianca", levantandoCrianca);
        anim.SetBool("soltandoCrianca", soltandoCrianca);
        anim.SetBool("usandoHabilidade", usandoHabilidade);
        anim.SetBool("atacando", atacando);
    }
}
