using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Personagem : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 movimento;
    private InputAction interactAction;
    public int velocidade = 600;
    public QuickTimeManager qteManager;

    public GameObject objetoInterativo;
    public float distanciaMaxima = 3.0f;


    public Slider progressBar;
    private bool isInteracting = false;
    private float interactionProgress = 0f;
    private float interactionDuration = 20f;
    public int vidas = 2;
    private bool isDead = false;





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
        if (context.performed && !isInteracting && !isDead)
        {
            float distancia = Vector3.Distance(transform.position, objetoInterativo.transform.position);
            if (distancia <= distanciaMaxima)
            {
                Debug.Log("Iniciando interação...");
                isInteracting = true;
            }
            else
            {
                Debug.Log("Você está muito longe para interagir com o objeto.");
            }
        }
    }

    public void ReceberDano(int quantidade)
    {
        if (!isDead)
        {
            vidas -= quantidade;
            if (vidas <= 0)
            {
                isDead = true;
                Debug.Log("Você morreu!");
                velocidade = 350;
                
            }
        }
    }





    private void Update()
    {
        if (isInteracting)
        {
            interactionProgress += Time.deltaTime * fillRate;
            progressBar.value = Mathf.Clamp01(interactionProgress / interactionDuration);

            if (interactionProgress >= interactionDuration)
            {
                Debug.Log("Interação concluída!");
                // Execute sua lógica de interação aqui
                isInteracting = false;
                interactionProgress = 0f;
                progressBar.value = 0f;
               
                qteManager.IniciarQTE();


            }
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(movimento.x, 0, movimento.y) * Time.fixedDeltaTime * velocidade);
    }
}
