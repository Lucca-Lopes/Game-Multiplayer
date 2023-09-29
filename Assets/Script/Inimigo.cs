using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
using System;
//using static UnityEngine.Rendering.DebugUI;

public class Inimigo : NetworkBehaviour
{
    public float distanciaAtaque = 2.0f;
    public int dano = 1;
    public int velocidade = 600;
    private InputAction interactaction;
    private Vector2 movimento;
    private Vector2 mouseInput;
    private Rigidbody rb;
    private Personagem jogador;
    private InputAction interactAction;
    private Personagem jogadorCarregado;
    private  GameManager gameManager;
    public ulong networkObjectId;

    public float distanciaCarregamento = 2.0f;
    [SerializeField] private CinemachineFreeLook vc;
    //[SerializeField] EfeitoVisual efeitoScript;
    private Personagem jogadorSendoCarregado;

    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] TMPro.TextMeshProUGUI displayName;

    [SerializeField] ParticleSystem efeito;

    private void Awake()
    {
        interactAction = new InputAction("Interact", binding: "<Keyboard>/space");
        interactAction.performed += CarregarJogador;
        interactAction.canceled += LiberarJogador; // Adicione este seção para o evento "canceled"

        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        if (IsOwner)
        {
            nomeJogador.Value = GameManager.PlayerName;
        }
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
    }

    public void SetMouseInput(InputAction.CallbackContext value)
    {
        mouseInput = value.ReadValue<Vector2>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
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
    }

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        displayName.text = current.ToString();
    }
    [ClientRpc]
    private void InformarCarregamentoClientRpc(ulong jogadorId, bool carregando)
    {
        if (IsOwner)
        {
            GameManager.Instance.JogadorCarregado(jogadorId);
            if (!carregando)
            {
                GameManager.Instance.JogadorLiberado(jogadorId);
            }
        }
    }


    public void CarregarJogador(InputAction.CallbackContext value)
    {
        if (jogador.vidas <= 0 && value.started)
        {
            float distanciaJogadorInimigo = Vector3.Distance(transform.position, jogador.transform.position);
            if (distanciaJogadorInimigo <= distanciaCarregamento)
            {
                Personagem player = FindObjectOfType<Personagem>();
                if (player != null)
                {
                    player.SerCarregadoPorInimigo(this);
                    velocidade = 800;

                    // Notificar o GameManager que o jogador está sendo carregado
                    InformarCarregamentoClientRpc(player.OwnerClientId, true);
                }
            }
        }
    }

    public void LiberarJogador(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            Personagem player = FindObjectOfType<Personagem>();
            if (player != null)
            {
                player.transform.SetParent(jogador.previousParent);
                jogador.isBeingCarried = false;
                velocidade = 600;

                Vector3 offset = transform.forward * 2.0f;
                player.transform.position = transform.position + offset;

                player.velocidade = 350;

                // Notificar o GameManager que o jogador foi liberado
                InformarCarregamentoClientRpc(player.OwnerClientId, false);
            }
        }
    }




    private void FixedUpdate()
    {
        if (rb != null)
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

    public void Atacar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsOwner)
            {
                Debug.Log("Atacou");
                efeito.Play();
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, distanciaAtaque);
                foreach (Collider col in hitColliders)
                {
                    if (col.gameObject.CompareTag("Sobrevivente"))
                    {
                        GameManager.Instance.CausarDano_ServerRpc(1, col.GetComponent<Personagem>().OwnerClientId);
                    }
                }
            }
        }
    }

    public static implicit operator Inimigo(Personagem v)
    {
        throw new NotImplementedException();
    }
}
