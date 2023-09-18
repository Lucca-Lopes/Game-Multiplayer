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
    public bool isBeingCarried = false;
    private Inimigo carryingEnemy;
    public Transform previousParent;

    public float fillRate = 0.05f;
    public void SerCarregadoPorInimigo(Inimigo enemy)
    {
        isBeingCarried = true;
        carryingEnemy = enemy;
        previousParent = transform.parent; 
        transform.SetParent(enemy.transform);
        //this.rb.isKinematic = true;
                                              
        velocidade = 200;
    }


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
                Debug.Log("Iniciando intera��o...");
                progressBar.gameObject.SetActive(true);
                isInteracting = true;
            }
            else
            {
                Debug.Log("Voc� est� muito longe para interagir com o objeto.");
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
                Debug.Log("Voc� morreu!");
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
                Debug.Log("Intera��o conclu�da!");
                
                isInteracting = false;
                progressBar.gameObject.SetActive(false);
                interactionProgress = 0f;
                progressBar.value = 0f;
               
                qteManager.IniciarQTE();


            }
        }
    }
    private void FixedUpdate()
    {
        if (isBeingCarried)
        {
            Vector3 desiredPosition = carryingEnemy.transform.position + Vector3.up * 2.01f;
            rb.MovePosition(desiredPosition);
        }
        else
        {
           rb.AddForce(new Vector3(movimento.x, 0, movimento.y) * Time.fixedDeltaTime * velocidade);
        }
    }

}
