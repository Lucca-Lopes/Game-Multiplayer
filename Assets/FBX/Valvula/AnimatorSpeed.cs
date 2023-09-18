using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSpeed : MonoBehaviour
{
    public Animator anim;

    private void Start()
    {
        anim.speed = 0;
    }
    public void AnimationSpeed(float speed)
    {
        anim.speed = speed;
    }
}
