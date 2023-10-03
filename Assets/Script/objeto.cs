using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objeto : MonoBehaviour
{
    public Slider progressBar;
    private Personagem jogador;

    public void Awake()
    {
        if (jogador != null)
        {
            jogador = GetComponent<Personagem>();

        }

    }
    public void StartInteraction()
    {
        Debug.Log("Iniciando interação...");
        progressBar.gameObject.SetActive(true);

        // Use o operador GetComponent() para obter uma referência ao objeto jogador.
        jogador = GetComponent<Personagem>();

        jogador.isInteracting = true;

        jogador.wasInteractingBeforeMoving = true;

        jogador.isAnyPlayerInteracting = true;
    }
    public  void ResumeInteraction()
    {
        Debug.Log("Retomando interação...");
        progressBar.gameObject.SetActive(true);
        jogador.isInteracting = true;
        jogador.isAnyPlayerInteracting = true;


       jogador.wasInteractingBeforeMoving = true;


        progressBar.value = jogador.progressBarPositionBeforeInterrupt;
    }
   
}
