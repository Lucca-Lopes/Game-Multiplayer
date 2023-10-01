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
    public int velocidade = 600;
    public GameManager gameManager;
    //public QuickTimeManager qteManager;

    public NetworkVariable<int> vidaJogador = new(2);
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] TMPro.TextMeshProUGUI displayName;

    public GameObject objetoInterativo;
    public float distanciaMaxima = 3.0f;


    public Slider progressBar;
    private bool isInteracting = false;
    private float interactionProgress = 0f;
    private float interactionDuration = 20f;
    public int vidas = 2;
    public bool isDead = false;
    public NetworkVariable <bool> isBeingCarried =new (false);
    public NetworkVariable<Inimigo> carryingEnemy =new();
    public Inimigo enemy;
    public Transform previousParent;
    [SerializeField] private CinemachineFreeLook vc;
    //[SerializeField] private AudioListener listener;
    public float fillRate = 0.05f;

    public void SerCarregadoPorInimigo(Inimigo enemy)
    {
        isBeingCarried.Value = true;
        carryingEnemy.Value = enemy;
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

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        displayName.text = current.ToString();
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
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

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && !isInteracting && !isDead)
        {
            float distancia = Vector3.Distance(transform.position, objetoInterativo.transform.position);
            if (distancia <= distanciaMaxima)
            {
                Debug.Log("Iniciando interação...");
                progressBar.gameObject.SetActive(true);
                isInteracting = true;

                // Chame a lógica de carregamento do jogador inimigo
                if (carryingEnemy != null)
                {
                    carryingEnemy.Value.CarregarJogador(context);
                }
            }
            else
            {
                Debug.Log("Você está muito longe para interagir com o objeto.");
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
        if (isBeingCarried.Value)
        {
            Vector3 desiredPosition = carryingEnemy.Value.transform.position + Vector3.up * 2.01f;
            rb.MovePosition(desiredPosition);
        }
        else
        {
            // Calcular a direção com base na rotação atual
            Vector3 moveDirection = Quaternion.Euler(0, vc.State.CorrectedOrientation.eulerAngles.y, 0) * new Vector3(movimento.x, 0, movimento.y);

            // Aplicar uma força na direção calculada
            rb.AddForce(moveDirection.normalized * Time.fixedDeltaTime * velocidade);
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
