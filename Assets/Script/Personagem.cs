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
    [Header("Assemblies")]
    [SerializeField] TextMeshProUGUI lobbyText;
    [SerializeField] private CinemachineFreeLook vc;
    [SerializeField] GameObject playerCam;
    [SerializeField] TextMeshProUGUI displayName;
    [SerializeField] Transform displayCanvas;
    [SerializeField] AnimationManager animations;
    public VisualEffect zzz;

    [Header("Configurações")]
    public int velocidade = 4;
    [SerializeField] float gravityValue = -9.81f;
    float verticalVelocity;
    Vector3 move = Vector3.zero;
    public bool jogoIniciado = false;
    public bool isDead = false;

    [Header("Sonorização")]
    public float spatialBlendValue = 1f; // Define a mistura espacial do áudio
    [SerializeField] private float minDistance = 5f; // Defina a distância mínima em que o áudio é ouvido claramente
    [SerializeField] private float maxDistance = 10f; // Defina a distância máxima em que o áudio é ouvido
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic; // Defina o rolloff mode para Logarítmico
    [SerializeField] private AudioClip somCaminhando;
    private AudioSource audioSource; // Será usado para reproduzir o som

    [Header("NetVars")]
    public NetworkVariable<int> vidaJogador = new(2);
    public NetworkVariable<int> pontucaoJogador = new(0);
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    //variáveis internas
    private CharacterController controller;
    private Vector2 movimento;
    private Vector2 mouseInput;

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
        #region spam de som
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = somCaminhando;
        audioSource.loop = true; // Isso define o som para reprodução contínua
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.spatialBlend = spatialBlendValue;
        audioSource.rolloffMode = rolloffMode;
        #endregion spam de som
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
                if (current == 2)
                {
                    isDead = false;
                    animations.dormindo.Value = false;
                    animations.sonolento.Value = false;
                }
                else if (current == 1)
                {
                    isDead = false;
                    animations.dormindo.Value = false;
                    animations.sonolento.Value = true;
                }
                else if (current == 0)
                {
                    isDead = true;
                    animations.dormindo.Value = true;
                    animations.sonolento.Value = true;
                }
            }
        }
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        if (IsOwner)
        {
            movimento = value.ReadValue<Vector2>();
        }
    }

    #region Rpcs de som
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
    #endregion Rpcs de som

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
                if (audioSource.isPlaying)
                    parandosomandando_ServerRpc();
            }
        }
    }
}
