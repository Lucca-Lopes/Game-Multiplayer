using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using Cinemachine;
using static UnityEngine.Rendering.DebugUI;
using TMPro;
using UnityEngine.VFX;

public class Personagem : NetworkBehaviour
{
    //Variáveis de movimento
    //private Rigidbody rb;
    private CharacterController controller;
    private Vector2 movimento;
    private Vector2 mouseInput;
    public int velocidade = 4;
    float verticalVelocity;
    [SerializeField] float gravityValue = -9.81f;
    Vector3 move = Vector3.zero;
    [SerializeField] private AudioClip somCaminhando;
    private AudioSource audioSource; // Será usado para reproduzir o som

    //Variável para controle do lobby
    [SerializeField] TextMeshProUGUI lobbyText;
    bool jogoIniciado = false;

    //Variáveis do Netcode
    public NetworkVariable<int> vidaJogador = new(1);
    public NetworkVariable<int> pontucaoJogador = new(0);
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Variável para o Nome
    [SerializeField] TextMeshProUGUI displayName;
    [SerializeField] Transform displayCanvas;

    //Variável para animação
    [SerializeField] AnimationManager animations;
    public VisualEffect zzz;

    //Variável para controle de vida
    public bool isDead = false;

    //Variável para controle de câmera
    [SerializeField] private CinemachineFreeLook vc;
    [SerializeField] GameObject playerCam;
    public float spatialBlendValue = 1f; // Define a mistura espacial do áudio
    [SerializeField] private float minDistance = 5f; // Defina a distância mínima em que o áudio é ouvido claramente
    [SerializeField] private float maxDistance = 10f; // Defina a distância máxima em que o áudio é ouvido
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic; // Defina o rolloff mode para Logarítmico

    //COISAS ANTIGAS
    //[SerializeField] private AudioListener listener;

    private void Awake()
    {
        //rb = GetComponent<Rigidbody>(); 
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (IsOwner)
        {
            nomeJogador.Value = PlayerData.playerName;
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = somCaminhando;
        audioSource.loop = true; // Isso define o som para reprodução contínua
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.spatialBlend = spatialBlendValue;
        audioSource.rolloffMode = rolloffMode;
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            vidaJogador.OnValueChanged += OnLifeChanged;
            nomeJogador.OnValueChanged += OnPlayerNameChanged;
            UpdateDisplayName(nomeJogador.Value.ToString());
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
        if (IsClient)
        {
            vidaJogador.OnValueChanged -= OnLifeChanged;
            nomeJogador.OnValueChanged -= OnPlayerNameChanged;
        }
    }

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        UpdateDisplayName(current.ToString());
    }

    void UpdateDisplayName(string value)
    {
        displayName.text = value;
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
                    animations.sonolento.Value = true;
                    animations.dormindo.Value = true;
                    zzz.Play();
                    AtualizarPontuacao_ServerRpc();
                }
            }

            if (current > previous)
            {
                isDead = false;
                animations.dormindo.Value = false;
            }
            if (previous > current)
            {
                animations.sonolento.Value = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AtualizarPontuacao_ServerRpc()
    {
        pontucaoJogador.Value = 300 - (int)TimerController.timer;
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        if (IsOwner /*&& !isDead && GameManager.Instance.timerAtivo.Value*/)
        {
            movimento = value.ReadValue<Vector2>();
            //Debug.Log($"movimento = {value}");
            /*if (value.ReadValue<Vector2>() != Vector2.zero)
                animations.correndo.Value = true;
            else
                animations.correndo.Value = false;*/
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void andandoSom_ServerRpc()
    { 
       andandoSom_ClientRpc();
    }
    [ClientRpc]
    private void andandoSom_ClientRpc()
    {
        audioSource.Play();
        //Debug.Log("Andando client");
    }
    [ServerRpc(RequireOwnership = false)]
    private void parandosomandando_ServerRpc()
    {
        parandosomandando_ClientRpc();
    }
    [ClientRpc]
    private void parandosomandando_ClientRpc()
    {
        audioSource.Stop();
        //Debug.Log("parando host");
    }
    public void SetMouseInput(InputAction.CallbackContext value)
    {
        mouseInput = value.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (IsClient)
        {
            if (GameManager.Instance.timerAtivo.Value && !jogoIniciado)
            {
                lobbyText.gameObject.SetActive(false);
                jogoIniciado = true;
            }
            else if (!GameManager.Instance.timerAtivo.Value && !jogoIniciado)
            {
                lobbyText.gameObject.SetActive(true);
            }
        }

        if (IsOwner)
        {
            if (GameManager.Instance.timerAtivo.Value && !isDead)
            {
                //movimento por character controller
                if (controller.isGrounded && verticalVelocity < 0)
                    verticalVelocity = 0f;

                Vector3 cameraForward = playerCam.transform.forward;
                cameraForward.y = 0;
                cameraForward = cameraForward.normalized;
                Vector3 cameraRight = playerCam.transform.right;
                cameraRight.y = 0;
                if (movimento != Vector2.zero && !audioSource.isPlaying)
                {
                    andandoSom_ServerRpc();
                }
                else if (movimento == Vector2.zero && audioSource.isPlaying)
                {
                    parandosomandando_ServerRpc();
                }

                Vector3 moveDirectionForward = cameraForward * movimento.y;
                Vector3 moveDirectionSideways = cameraRight * movimento.x;
                Vector3 moveDirection = (moveDirectionForward + moveDirectionSideways);

                move = moveDirection * velocidade;

                if (move != Vector3.zero)
                {
                    gameObject.transform.forward = move;
                    animations.correndo.Value = true;
                }
                else
                {
                    animations.correndo.Value = false;
                }

                verticalVelocity += gravityValue * Time.deltaTime;
                move.y = verticalVelocity;
                controller.Move(move * Time.deltaTime);
            }
            else
            {
                move = Vector3.zero;
                animations.correndo.Value = false;
                parandosomandando_ServerRpc();
            }
        }
    }

    /*private void FixedUpdate()
    {
        if (!isDead)
        {
            // Calcular a direção com base na rotação atual
            Vector3 moveDirection = Quaternion.Euler(0, vc.State.CorrectedOrientation.eulerAngles.y, 0) * new Vector3(movimento.x, 0, movimento.y);

            // Aplicar uma força na direção calculada
            rb.AddForce(moveDirection.normalized * Time.fixedDeltaTime * velocidade);
            if (moveDirection != Vector3.zero)
                gameObject.transform.forward = moveDirection.normalized;
        }
        //RotateWithMouseInput();
    }*/

    /*private void RotateWithMouseInput()
    {
        // Obter a rotação atual da câmera
        Quaternion cameraRotation = vc.State.CorrectedOrientation;

        // Converter o input do mouse em uma rotação local
        Vector3 localRotation = new Vector3(-mouseInput.y, mouseInput.x, 0);

        // Aplicar a rotação local à rotação da câmera
        Quaternion newRotation = cameraRotation * Quaternion.Euler(localRotation);

        // Definir a rotação do personagem para a nova rotação
        transform.rotation = newRotation;
    }*/
}
