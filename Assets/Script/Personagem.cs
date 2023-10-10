using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using Cinemachine;
using static UnityEngine.Rendering.DebugUI;

public class Personagem : NetworkBehaviour
{
    private Rigidbody rb;
    private Vector2 movimento;
    private Vector2 mouseInput;
    private InputAction interactAction;
    public float speedMultiplier = 6.0f;

    public GameManager gameManager;
    //public QuickTimeManager qteManager;

    public NetworkVariable<int> vidaJogador = new(2);
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] TMPro.TextMeshProUGUI displayName;

    public GameObject objetoInterativo;
    public float distanciaMaxima = 3.0f;


    public Slider progressBar;
     internal bool isInteracting = false;
    public  float interactionProgress = 0f;
    internal float interactionDuration = 20f;
    public int vidas = 2;
    public bool isDead = false;
    public NetworkVariable <bool> isBeingCarried =new (false);
    public Inimigo carryingEnemy;
    public Inimigo enemy;
    public objeto obj;
    public static Personagem Instance;
    private objeto objetoInteragindo;

    public Transform previousParent;
    public bool isAnyPlayerInteracting =false;
    [SerializeField] private CinemachineFreeLook vc;
    public   bool wasInteractingBeforeMoving =false;
    public bool isMoving = false;
    public  float progressBarPositionBeforeInterrupt = 0f;
    private bool isMovingBeforeInteraction = false;
    public objeto objeto;
    private GameObject progressBarObject;
    //[SerializeField] private AudioListener listener;
    public float fillRate = 0.05f;
    public delegate void InteractionHandler();
    public static event InteractionHandler OnInteractionRequested;
    public void SerCarregadoPorInimigo(Inimigo enemy)
    {
       
        isBeingCarried.Value = true;
        carryingEnemy= enemy;
        previousParent = transform.parent;
        transform.SetParent(enemy.transform);
        //this.rb.isKinematic = true;

        speedMultiplier = 3.0f;
        
    }



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        interactAction.performed += Interact;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        

    }

    private void Start()
    {
        if (IsOwner)
        {
            nomeJogador.Value = GameManager.PlayerName;

        }

    }
  
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            vidaJogador.OnValueChanged += OnLifeChanged;
            nomeJogador.OnValueChanged += OnPlayerNameChanged;
            displayName.text = nomeJogador.Value.ToString();
        }
        if (IsOwner)
        {
       // listener.enabled = true;
            vc.Priority = 10;
        }
        else
        {
            vc.Priority = 0;
        }
    }

    public override void OnNetworkDespawn()
    {
        nomeJogador.OnValueChanged -= OnPlayerNameChanged;
        vidaJogador.OnValueChanged -= OnLifeChanged;
    }


    void OnLifeChanged(int previous, int current)
    {
        if (!isDead)
        {
            if (current <= 0)
            {
                isDead = true;
                Debug.Log("Voce morreu!");
                 speedMultiplier= 3.0f;
            }
        }
        else
        {
            if(current > 0)
            {
                isDead = false;
                Debug.Log("Voce reviveu!");
                speedMultiplier = 3.0f;
            }
        }
    }

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        displayName.text = current.ToString();
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
        // Verifique se o movimento é não nulo (ou seja, o jogador está se movendo)
        isMoving = movimento.magnitude > 0;
        
    }


    public void SetMouseInput(InputAction.CallbackContext value)
    {
        mouseInput = value.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead)
        {
            GameObject[] objetosInterativos = GameObject.FindGameObjectsWithTag("objeto");

            if (objetosInterativos.Length == 0)
            {
                Debug.Log("Nenhum objeto interativo encontrado com a tag especificada.");
                return;
            }

            GameObject objetoMaisProximo = null;
            float distanciaMaisProxima = float.MaxValue;

            foreach (GameObject objeto in objetosInterativos)
            {
                float distancia = Vector3.Distance(transform.position, objeto.transform.position);
                if (distancia < distanciaMaxima && distancia < distanciaMaisProxima)
                {
                    objetoMaisProximo = objeto;
                    distanciaMaisProxima = distancia;
                }
            }

            if (objetoMaisProximo != null)
            {
                if (isAnyPlayerInteracting)
                {
                    Debug.Log("Outro jogador já está interagindo.");
                    return;
                }

                objetoInteragindo = objetoMaisProximo.GetComponent<objeto>();

                if (objetoInteragindo.isInteracting)
                {
                    // Chame ResumeInteraction no objeto
                    objetoInteragindo.ResumeInteraction();
                }
                else
                {
                    // Chame StartInteraction no objeto
                    objetoInteragindo.StartInteraction();
                }
            }
            else
            {
                Debug.Log("Você está muito longe para interagir com qualquer objeto.");
            }
        }
    }




    /*
    private void StartInteraction()
    {
        Debug.Log("Iniciando interação...");
        progressBar.gameObject.SetActive(true);
        isInteracting = true;

        if (!wasInteractingBeforeMoving)
        {
            interactionProgress = 0f;
        }

        wasInteractingBeforeMoving = true; // Adicione esta linha
        isAnyPlayerInteracting = true;
    }




    private void ResumeInteraction()
    {
        Debug.Log("Retomando interação...");
        progressBar.gameObject.SetActive(true);
        isInteracting = true;
        isAnyPlayerInteracting = true;

       
        wasInteractingBeforeMoving = true;

      
        progressBar.value = progressBarPositionBeforeInterrupt;
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
    */


    private void Update()
    {
        if (objetoInteragindo != null && objetoInteragindo.isInteracting)
        {
            float distanciaJogadorObjeto = Vector3.Distance(transform.position, objetoInteragindo.transform.position);
            if (distanciaJogadorObjeto > objetoInteragindo.maxInteractionDistance)
            {
                Debug.Log("Você se afastou do objeto interativo.");
                objetoInteragindo.StopInteraction(); // Crie um método StopInteraction no script do objeto para interromper a interação.
                objetoInteragindo = null; // Limpe a referência ao objeto interativo.
            }
        }

        // Resto do código do método Update
    }








    private void FixedUpdate()
    {
        if (carryingEnemy == null)
        {
            carryingEnemy = FindObjectOfType<Inimigo>();
        }

        if (isBeingCarried.Value)
        {
            rb.MovePosition(carryingEnemy.transform.position);

            Vector3 offset = carryingEnemy.transform.position - rb.position;
            offset.y = 2;
            rb.MovePosition(rb.position + offset);
        }
        else
        {
            // Calcular a direção com base na rotação atual
            Vector3 moveDirection = Quaternion.Euler(0, vc.State.CorrectedOrientation.eulerAngles.y, 0) * new Vector3(movimento.x, 0, movimento.y);

            // Mover o personagem usando Translate()
            transform.Translate(moveDirection.normalized * Time.fixedDeltaTime * speedMultiplier, Space.World);
        }

        RotateWithMouseInput();
    }





    private void RotateWithMouseInput()
    {
        // Obter a rotação atual da câmera
        Quaternion cameraRotation = vc.State.CorrectedOrientation;

        // Converter o input do mouse em uma rotação local
        Vector3 localRotation = new Vector3(-mouseInput.y, mouseInput.x, 0);

        // Aplicar a rotação local à rotação da câmera
        Quaternion newRotation = cameraRotation * Quaternion.Euler(localRotation);

        // Definir a rotação do personagem para a nova rotação
        transform.rotation = newRotation;
    }

}
