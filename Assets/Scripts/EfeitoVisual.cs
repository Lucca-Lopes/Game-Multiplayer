using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfeitoVisual : MonoBehaviour
{
    [SerializeField] ParticleSystem efeito;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            efeito.Play();

        }
    }
}
