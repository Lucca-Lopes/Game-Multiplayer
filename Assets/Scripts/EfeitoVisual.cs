using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfeitoVisual : MonoBehaviour
{
    [SerializeField] ParticleSystem efeito;

    // Update is called once per frame
    private void Update()
    {
        efeito.Play();
    }
    public void AtivarEfeito()
    {
        efeito.Play();
    }
}
