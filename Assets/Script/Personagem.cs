using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using Cinemachine;

public class Personagem : NetworkBehaviour
{
    private Rigidbody rb;
    private Vector2 movimento;
    private InputAction interactAction;
    public int velocidade = 600;
    //public QuickTimeManager qteManager;

    public NetworkVariable<int> vidaJogador = new(2);

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
    [SerializeField] private CinemachineVirtualCamera vc;
    //[SerializeField] private AudioListener listener;
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

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            vidaJogador.OnValueChanged += OnLifeChanged;
        }
        if(IsOwner)
        {
       // listener.enabled = true;
            vc.Priority = 10;
        }
        else
        {
            vc.Priority = 0;
        }
    }

    void OnLifeChanged(int previous, int current)
    {
        if (!isDead)
        {
            if (current <= 0)
            {
                isDead = true;
                Debug.Log("Voce morreu!");
                velocidade = 350;
            }
        }
        else
        {
            if(current > 0)
            {
                isDead = false;
                Debug.Log("Voce reviveu!");
                velocidade = 600;
            }
        }
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

    //public void ReceberDano(int quantidade)
    //{
    //    if (!isDead)
    //    {
    //        vidas -= quantidade;
    //        if (vidas <= 0)
    //        {
    //            isDead = true;
    //            Debug.Log("Voc� morreu!");
    //            velocidade = 350;

    //        }
    //    }
    //}


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
               
                //qteManager.IniciarQTE();


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
