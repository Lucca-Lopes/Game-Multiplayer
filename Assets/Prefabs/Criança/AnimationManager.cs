using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator anim;
    public bool correndo;
    public bool sonolento;
    public bool flutuando;
    public bool dormindo;
    public bool hit;
    public bool interagindo;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetBool("Correndo", correndo);
        anim.SetBool("Sonolento", sonolento);
        anim.SetBool("Flutuando", flutuando);
        anim.SetBool("Dormindo", dormindo);
        anim.SetBool("Hit", hit);
        anim.SetBool("Interagindo", interagindo);
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
