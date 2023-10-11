using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objeto : MonoBehaviour
{
    public Slider progressBar;
    private GameManager gamemanager;
    private Personagem jogador;
    public float fillRate = 0.05f;
    public float interactionProgress = 0f;
    public bool isInteracting = false;
    public bool wasInteractingBeforeMoving = false;
    public bool isAnyPlayerInteracting = false;
    private bool isInteractionAvailable = true;
    public float maxInteractionDistance = 3f;
    private float interactionDuration = 90f; // Defina a dura��o da intera��o em segundos
    private float currentTime = 0f; // Tempo atual da intera��o
    private float pausedInteractionProgress = 0f;
    private bool wasInteractingBeforePause = false;

    private GameObject jogadors;

    public void SetJogador(Personagem player)
    {
        jogador = player;
    }

    [SerializeField]
    public int objectsCompleted;

    void Start()
    {
        jogadors = GameObject.Find("Player");
    }

    public void StartInteraction()
    {
        if (isInteractionAvailable)
        {
            Debug.Log("Iniciando intera��o...");
            progressBar.gameObject.SetActive(true);

            // Verifique se o jogador estava interagindo antes da pausa
            if (wasInteractingBeforePause)
            {
                // Se estava, retome a partir do progresso salvo
                interactionProgress = pausedInteractionProgress;
                progressBar.value = interactionProgress;
            }
            else
            {
                // Se n�o estava, comece do in�cio
                interactionProgress = 0f;
            }

            isInteracting = true;
            wasInteractingBeforeMoving = true;
            isAnyPlayerInteracting = true;
        }
        else
        {
            Debug.Log("A intera��o n�o est� dispon�vel.");
        }
    }


    public void StopInteraction()
    {
        Debug.Log("Intera��o interrompida pelo jogador.");

        // Salve o progresso da intera��o antes de interromper
        pausedInteractionProgress = interactionProgress;
        wasInteractingBeforePause = wasInteractingBeforeMoving;

        progressBar.gameObject.SetActive(false);
        interactionProgress = 0f;
        isInteracting = false;
        wasInteractingBeforeMoving = false;
        isAnyPlayerInteracting = false;
    }


    public void ResumeInteraction()
    {
        Debug.Log("Retomando intera��o...");
        progressBar.gameObject.SetActive(true);
        isInteracting = true;
        isAnyPlayerInteracting = true;
        wasInteractingBeforeMoving = true;
    }

    void Update()
    {
        if (isInteracting)
        {
            if (!jogador || !jogador.isMoving)
            {
                currentTime += Time.deltaTime;

                // Calcule o progresso da intera��o com base no tempo atual
                interactionProgress = currentTime / interactionDuration;

                interactionProgress = Mathf.Clamp01(interactionProgress);
                progressBar.value = interactionProgress;

                if (currentTime >= interactionDuration)
                {
                    Debug.Log("Intera��o conclu�da!");
                    isInteracting = false;
                    progressBar.gameObject.SetActive(false);
                    interactionProgress = 0f;
                    objectsCompleted += 1;
                    GameManager.Instance.objetivoscompletadosServerRpc();
                    isInteractionAvailable = false;
                }
            }
            else
            {
                Debug.Log("Intera��o interrompida devido ao movimento.");
                progressBar.gameObject.SetActive(false);
                interactionProgress = 0f;
                isInteracting = false;
                wasInteractingBeforeMoving = false;
                isAnyPlayerInteracting = false;

                // Redefina o tempo atual
                currentTime = 0f;
            }
        }
    }
}
