using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objeto : MonoBehaviour
{
    public Slider progressBar;
    private Personagem jogador;
    public float fillRate = 0.05f;
    public float interactionProgress = 0f;
    private bool isInteracting = false;
    public bool wasInteractingBeforeMoving = false;
    public bool isAnyPlayerInteracting = false;
    private GameObject jogadors;
    public void SetJogador(Personagem player)
    {
        jogador = player;
    }
 

    void Start()
    {
        jogadors = GameObject.Find("Player");
    }

    public void StartInteraction()
    {
        Debug.Log("Iniciando interação...");
        progressBar.gameObject.SetActive(true);

        // Initialize interaction progress to 0
        interactionProgress = 0f;

        // Set the player as interacting
        isInteracting = true;
        wasInteractingBeforeMoving = true;
        isAnyPlayerInteracting = true;
    }

    public void ResumeInteraction()
    {
        Debug.Log("Retomando interação...");
        progressBar.gameObject.SetActive(true);

        // Set the player as interacting
        isInteracting = true;
        isAnyPlayerInteracting = true;
        wasInteractingBeforeMoving = true;

        // Set the progress bar's value to the saved progress
        progressBar.value = jogador.progressBarPositionBeforeInterrupt;
    }

    private void Update()
    {
        if (isInteracting)
        {
            if (!jogador || !jogador.isMoving) // Check if the character is not moving and the player object exists
            {
                // Update the interaction progress
                interactionProgress += Time.deltaTime * fillRate;

                // Limit the progress to a range of 0 to 1
                interactionProgress = Mathf.Clamp01(interactionProgress);

                // Update the progress bar value
                progressBar.value = interactionProgress;

                if (interactionProgress >= 1f)
                {
                    // Interaction completed
                    Debug.Log("Interação concluída!");

                    // Reset interaction state
                    isInteracting = false;
                    progressBar.gameObject.SetActive(false);
                    interactionProgress = 0f;
                }
            }
            else
            {
                // The character is moving, stop the interaction
                Debug.Log("Interação interrompida devido ao movimento.");
                progressBar.gameObject.SetActive(false);
                interactionProgress = 0f;
                isInteracting = false;
                wasInteractingBeforeMoving = false;
                isAnyPlayerInteracting = false;
            }
        }
    }

}

