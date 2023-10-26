using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
using System.Collections;
using TMPro;
//using static UnityEngine.Rendering.DebugUI;

public class Inimigo : NetworkBehaviour
{
    [Header("Assemblies")]
    [SerializeField] AnimationEvents animations;
    [SerializeField] private CinemachineFreeLook vc;
    [SerializeField] ParticleSystem efeito;
    [SerializeField] TextMeshProUGUI lobbyText;
    //[SerializeField] EfeitoVisual efeitoScript;

    [Header("Configurações")]
    public float distanciaAtaque = 2.0f;
    public int dano = 1;
    public int velocidade = 600;
    public float distanciaCarregamento = 2.0f;

    [Header("NetVars")]
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Variável para o Nome
    [SerializeField] TextMeshProUGUI displayName;

    //variáveis internas
    private Vector2 movimento;
    private Vector2 mouseInput;
    private Rigidbody rb;
    private bool canWalk = true;
    
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
            nomeJogador.OnValueChanged += OnPlayerNameChanged;
            nomeJogador.Value = GameManager.PlayerName;
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
        }
    }

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        displayName.text = current.ToString();
    }

    private void Update()
    {
        if (IsClient)
        {
            if (GameManager.Instance.timerAtivo.Value)
            {
                lobbyText.gameObject.SetActive(false);
                canWalk = true;
            }
            else
            {
                lobbyText.gameObject.SetActive(true);
                canWalk = false;
            }
        }
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        if (GameManager.Instance.timerAtivo.Value && IsOwner && canWalk)
        {
            movimento = value.ReadValue<Vector2>();
            if (value.ReadValue<Vector2>() != Vector2.zero)
                animations.andando.Value = true;
            else
                animations.andando.Value = false;
        }
    }

    public void SetMouseInput(InputAction.CallbackContext value)
    {
        mouseInput = value.ReadValue<Vector2>();
    }

    //Movimento usando Rigdbody
    private void FixedUpdate()
    {
        if (rb != null)
        {
            // Calcular a direção com base na rotação atual
            Vector3 moveDirection = Quaternion.Euler(0, vc.State.CorrectedOrientation.eulerAngles.y, 0) * new Vector3(movimento.x, 0, movimento.y);

            // Aplicar uma força na direção calculada
            rb.AddForce(moveDirection.normalized * Time.fixedDeltaTime * velocidade);
            gameObject.transform.forward = moveDirection.normalized;
        }
        //RotateWithMouseInput();
    }

    private void RotateWithMouseInput()
    {
        // Obter a rotação atual da câmera
        Quaternion cameraRotation = vc.State.CorrectedOrientation;

        // Converter o input do mouse em uma rotação local
        Vector3 localRotation = new Vector3(-mouseInput.y, mouseInput.x, 0);

        // Aplicar a rotação local à rotação da câmera
        Quaternion newRotation = cameraRotation * Quaternion.Euler(localRotation);
        newRotation.z = 0;
        // Definir a rotação do personagem para a nova rotação
        transform.rotation = newRotation;
    }

    public void Atacar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsOwner)
            {
                StartCoroutine(AnimacaoAtacar());
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

    IEnumerator AnimacaoAtacar()
    {
        canWalk = false;
        animations.atacando.Value = true;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(2.0f);
        animations.atacando.Value = false;
        canWalk = true;
    }
}
