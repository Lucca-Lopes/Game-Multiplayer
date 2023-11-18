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
    [SerializeField] GameObject playerCam;
    //[SerializeField] ParticleSystem efeito;
    [SerializeField] TextMeshProUGUI lobbyText;
    [SerializeField] private  AudioSource somDeAtaque;
    [SerializeField] audioevenbtes audioevents;
    //[SerializeField] EfeitoVisual efeitoScript;

    [Header("Configurações")]
    public float distanciaAtaque = 5f;
    [SerializeField] bool mostrarGizmos;
    public int dano = 1;
    public float velocidade = 4;
    float verticalVelocity;
    [SerializeField] float gravityValue = -9.81f;
    public float distanciaCarregamento = 2.0f;
    Vector3 move = Vector3.zero;

    [Header("NetVars")]
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Variável para o Nome
    [SerializeField] TextMeshProUGUI displayName;

    //variáveis internas
    private Vector2 movimento;
    private Vector2 mouseInput;
    //private Rigidbody rb;
    private CharacterController controller;
    private bool canWalk = true;
    bool jogoIniciado = false;
    public NetworkVariable<bool> atacando;
    private void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
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
            somDeAtaque.GetComponent<AudioSource>();
            atacando.OnValueChanged += ChangersomAtacando;
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
    void ChangersomAtacando(bool previous, bool current)
    {
        somDeAtaque.Play();
        Debug.Log("tocou o audio");
    }
    private void Update()
    {
        if (IsClient)
        {
            if (GameManager.Instance.timerAtivo.Value && !jogoIniciado)
            {
                lobbyText.gameObject.SetActive(false);
                jogoIniciado = true;
                canWalk = true;
            }
            else if (!GameManager.Instance.timerAtivo.Value && !jogoIniciado)
            {
                lobbyText.gameObject.SetActive(true);
                canWalk = false;
            }
        }
        if (canWalk && IsOwner)
        {
            //movimento por character controller
            if (controller.isGrounded && verticalVelocity < 0)
                verticalVelocity = 0f;

            Vector3 cameraForward = playerCam.transform.forward;
            cameraForward.y = 0;
            cameraForward = cameraForward.normalized;
            Vector3 cameraRight = playerCam.transform.right;
            cameraRight.y = 0;

            Vector3 moveDirectionForward = cameraForward * movimento.y;
            Vector3 moveDirectionSideways = cameraRight * movimento.x;
            Vector3 moveDirection = (moveDirectionForward + moveDirectionSideways);

            move = moveDirection * velocidade;

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
                animations.andando.Value = true;
            }
            else
                animations.andando.Value = false;

            verticalVelocity += gravityValue * Time.deltaTime;
            move.y = verticalVelocity;
            controller.Move(move * Time.deltaTime);
        }
        else if (IsOwner)
        {
            move = Vector3.zero;
            animations.andando.Value = false;
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

    /*//Movimento usando Rigdbody
    private void FixedUpdate()
    {
        if (rb != null)
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
        newRotation.z = 0;
        // Definir a rotação do personagem para a nova rotação
        transform.rotation = newRotation;
    }*/

    public void Atacar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsOwner && GameManager.Instance.timerAtivo.Value)
            {
                animations.atacando.Value = true;
                atacando.Value = true;
                canWalk = false;
                //Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1.367f + transform.up * 1.3f, distanciaAtaque);
                Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.forward * (distanciaAtaque / 2) + transform.up * 1.3f, new(3f,4f,distanciaAtaque));
                foreach (Collider col in hitColliders)
                {
                    if (col.gameObject.CompareTag("Sobrevivente"))
                    {
                        GameManager.Instance.CausarDano_ServerRpc(1, col.GetComponent<Personagem>().OwnerClientId);
                        Debug.Log($"Causando dano ao client {(int)col.GetComponent<Personagem>().OwnerClientId}");
                    }
                    
                        
                    
                }
            }
        }
    }

    public void FinishAttack()
    {
        if (IsOwner)
        {
            animations.atacando.Value = false;
            atacando.Value= false;
            canWalk = true;
        }
    }

    /*IEnumerator AnimacaoAtacar()
    {
        canWalk = false;
        animations.atacando.Value = true;
        //rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(2.0f);
        animations.atacando.Value = false;
        canWalk = true;
    }*/

    // Hitbox do Ataque
    private void OnDrawGizmos()
    {
        if (mostrarGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position + transform.forward * (distanciaAtaque / 2) + transform.up * 1.3f, new(3f, 4f, distanciaAtaque));
        }
    }
}
