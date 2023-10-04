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
    private bool isInteracting = false;
    public bool wasInteractingBeforeMoving = false;
    public bool isAnyPlayerInteracting = false;
    private bool isInteractionAvailable = true;

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
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
            }

            interactionProgress = 0f;
            isInteracting = true;
            wasInteractingBeforeMoving = true;
            isAnyPlayerInteracting = true;
        }
        else
        {
            Debug.Log("A intera��o n�o est� dispon�vel.");
        }
    }

    public void ResumeInteraction()
    {
        Debug.Log("Retomando intera��o...");
        progressBar.gameObject.SetActive(true);

        isInteracting = true;
        isAnyPlayerInteracting = true;
        wasInteractingBeforeMoving = true;

        progressBar.value = jogador.progressBarPositionBeforeInterrupt;
    }

    void Update()
    {
        if (isInteracting)
        {
            if (CheckDistance(jogador))
            {
                interactionProgress += Time.deltaTime * fillRate;
                interactionProgress = Mathf.Clamp01(interactionProgress);
                progressBar.value = interactionProgress;

                if (interactionProgress >= 1f)
                {
                    Debug.Log("Intera��o conclu�da!");

                    isInteracting = false;
                    progressBar.gameObject.SetActive(false);
                    interactionProgress = 0f;

                    objectsCompleted += 1;

                    isInteractionAvailable = false;
                }
            }
            else
            {
                Debug.Log("Intera��o pausada devido � dist�ncia.");
                progressBar.gameObject.SetActive(false);
                isInteracting = false;
                wasInteractingBeforeMoving = false;
            }
        }
    }

    private bool CheckDistance(Personagem jogador)
    {
        // Verificar se o jogador existe
        bool isPlayerValid = jogador != null;

        // Se o jogador existir, verifique a dist�ncia
        if (isPlayerValid)
        {
            // Verifique a dist�ncia entre o objeto e o jogador
            float distance = Vector3.Distance(transform.position, jogador.transform.position);

            // Adicione a altura do objeto e do jogador � dist�ncia
            distance += transform.position.y + (jogador?.transform.position.y ?? 0f);

            // Se a dist�ncia for menor que um determinado valor, ent�o o jogador est� na dist�ncia do objeto
            return distance < 3f;
        }

        // Se o jogador n�o existir, retorne false
        return false;

    }

}
