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
    private float interactionDuration = 90f; // Defina a duração da interação em segundos
    private float currentTime = 0f; // Tempo atual da interação
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
            Debug.Log("Iniciando interação...");
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
                // Se não estava, comece do início
                interactionProgress = 0f;
            }

            isInteracting = true;
            wasInteractingBeforeMoving = true;
            isAnyPlayerInteracting = true;
        }
        else
        {
            Debug.Log("A interação não está disponível.");
        }
    }


    public void StopInteraction()
    {
        Debug.Log("Interação interrompida pelo jogador.");

        // Salve o progresso da interação antes de interromper
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
        Debug.Log("Retomando interação...");
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

                // Calcule o progresso da interação com base no tempo atual
                interactionProgress = currentTime / interactionDuration;

                interactionProgress = Mathf.Clamp01(interactionProgress);
                progressBar.value = interactionProgress;

                if (currentTime >= interactionDuration)
                {
                    Debug.Log("Interação concluída!");
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
                Debug.Log("Interação interrompida devido ao movimento.");
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
