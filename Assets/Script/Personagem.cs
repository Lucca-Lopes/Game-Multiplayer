using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Personagem : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 movimento;
    private InputAction interactAction;
    public QuickTimeManager qteManager;

    public GameObject objetoInterativo;
    public float distanciaMaxima = 3.0f;

    [SerializeField] GameObject telaGameOver;
    [SerializeField] Text textoPlacarFinal;
    [SerializeField] TMP_Text textoPlacarTela;
    [SerializeField] WebRequest webRequest;

    public Slider progressBar;
    private bool isInteracting = false;
    private bool minigameEnded = false;
    private bool updatePlacar = false;
    private float interactionProgress = 0f;
    private float interactionDuration = 60f;

    // Ajuste esta taxa de preenchimento para controlar a velocidade da barra de progresso
    public float fillRate = 0.05f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        interactAction.performed += Interact;
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && !isInteracting)
        {
            float distancia = Vector3.Distance(transform.position, objetoInterativo.transform.position);
            if (distancia <= distanciaMaxima)
            {
                Debug.Log("Iniciando interação...");
                isInteracting = true;
                qteManager.IniciarQTE();
            }
            else
            {
                Debug.Log("Você está muito longe para interagir com o objeto.");
            }
        }
    }

    private void Update()
    {
        if (isInteracting)
        {
            interactionProgress += Time.deltaTime * fillRate;
            progressBar.value = Mathf.Clamp01(interactionProgress / interactionDuration);
            textoPlacarTela.text = "Pontuação: " + (QteStreak.score * 100).ToString();

            if (interactionProgress >= interactionDuration)
            {
                Debug.Log("Interação concluída!");
                // Execute sua lógica de interação aqui
                isInteracting = false;
                interactionProgress = 0f;
                progressBar.value = 0f;
                minigameEnded = true;
                


            }
        }

        if (minigameEnded)
        {
            telaGameOver.SetActive(true);
            textoPlacarFinal.text = (QteStreak.score * 100).ToString();
            textoPlacarTela.gameObject.SetActive(false);
            Time.timeScale = 0;
            if (!updatePlacar)
            {
                webRequest.UpdatePlayer();
                updatePlacar = true;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(300 * Time.fixedDeltaTime * new Vector3(-movimento.y, 0, movimento.x));
    }
}
