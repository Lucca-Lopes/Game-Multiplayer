using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using Cinemachine;
using static UnityEngine.Rendering.DebugUI;

public class Personagem : NetworkBehaviour
{
    //Variáveis de movimento
    private Rigidbody rb;
    private Vector2 movimento;
    private Vector2 mouseInput;
    private InputAction interactAction;
    public int velocidade = 600;

    //Variáveis do Netcode
    public NetworkVariable<int> vidaJogador = new(1);
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Variável para o Nome
    [SerializeField] TMPro.TextMeshProUGUI displayName;

    //Variável para animação
    [SerializeField] AnimationManager animations;

    //Variável para controle de vida
    public bool isDead = false;

    //Variável para controle de câmera
    [SerializeField] private CinemachineFreeLook vc;

    //COISAS ANTIGAS
    //[SerializeField] private AudioListener listener;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
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
        if (IsClient && IsOwner)
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
        if (IsOwner)
        {
            nomeJogador.OnValueChanged -= OnPlayerNameChanged;
            vidaJogador.OnValueChanged -= OnLifeChanged;
        }
    }


    void OnLifeChanged(int previous, int current)
    {
        if (IsOwner)
        {
            if (!isDead)
            {
                if (current <= 0)
                {
                    isDead = true;
                    Debug.Log("Personagem - OnLifeChanged - Voce morreu!");
                    velocidade = 0;
                    animations.dormindo = true;
                }
            }

            if (current > 0)
            {
                isDead = false;
                Debug.Log("Voce reviveu!");
                velocidade = 600;
                animations.dormindo = false;
            }
        }
    }

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        displayName.text = current.ToString();
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        if (IsOwner && !isDead)
        {
            movimento = value.ReadValue<Vector2>();
            if (value.performed)
                animations.correndo = true;
            if (value.canceled)
                animations.correndo = false;
        }
    }

    public void SetMouseInput(InputAction.CallbackContext value)
    {
        mouseInput = value.ReadValue<Vector2>();
    }

    //private void OnEnable()
    //{
    //    interactAction.Enable();
    //}

    //private void OnDisable()
    //{
    //    interactAction.Disable();
    //}

    private void FixedUpdate()
    {
        if (!isDead)
        {
            // Calcular a direção com base na rotação atual
            Vector3 moveDirection = Quaternion.Euler(0, vc.State.CorrectedOrientation.eulerAngles.y, 0) * new Vector3(movimento.x, 0, movimento.y);

            // Aplicar uma força na direção calculada
            rb.AddForce(moveDirection.normalized * Time.fixedDeltaTime * velocidade);

            RotateWithMouseInput();
        }
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
