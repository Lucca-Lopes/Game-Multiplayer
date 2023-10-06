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
    public  bool isInteracting = false;
    public bool wasInteractingBeforeMoving = false;
    public bool isAnyPlayerInteracting = false;
    private bool isInteractionAvailable = true; 
    public float maxInteractionDistance = 3f;

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
    public void StopInteraction()
    {
        Debug.Log("Intera��o interrompida pelo jogador.");
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
                    GameManager.Instance.objectsCompleted.Value+=1;


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
            }
        }
    }

}