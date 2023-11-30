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
    //[SerializeField] private  AudioSource somDeAtaque;
    
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
    private AudioSource risadaAudioSource; // adicione uma referência ao AudioSource do áudio de risada
    public AudioClip risadaClip; // adicione a clip de áudio de risada
    private AudioSource audioPassos;
    [SerializeField] private AudioClip audioPassosClip;
    [SerializeField] private AudioClip ataque;
    private AudioSource somataquesounce; // Será usado para reproduzir o som
    public float spatialBlendValue = 1f; // Define a mistura espacial do áudio
    [SerializeField] private float minDistance = 5f; // Defina a distância mínima em que o áudio é ouvido claramente
    [SerializeField] private float maxDistance = 10f; // Defina a distância máxima em que o áudio é ouvido
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic; // Defina o rolloff mode para Logarítmico


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
        risadaAudioSource = gameObject.AddComponent<AudioSource>(); // crie um AudioSource para a risada
        risadaAudioSource.clip = risadaClip; // atribua a clip de áudio de risada ao AudioSource
        risadaAudioSource.spatialBlend = spatialBlendValue;
        risadaAudioSource.minDistance = minDistance;
        risadaAudioSource.maxDistance = maxDistance;
        risadaAudioSource.rolloffMode = rolloffMode;
        if (IsOwner)
        {
            // outras inicializações...
            InvokeRepeating(nameof(ReproduzirRisada_ServerRpc), 0f, 5f); 
        }
        audioPassos = gameObject.AddComponent<AudioSource>();
        audioPassos.clip = audioPassosClip;
        audioPassos.loop = true; // Para reproduzir em loop enquanto se move
        somataquesounce = gameObject.AddComponent<AudioSource>();
        somataquesounce.clip = ataque;
        somataquesounce.spatialBlend = spatialBlendValue;
        somataquesounce.minDistance = minDistance;
        somataquesounce.maxDistance = maxDistance;
        somataquesounce.rolloffMode = rolloffMode;
        audioPassos.spatialBlend = spatialBlendValue;
        audioPassos.minDistance = minDistance;
        audioPassos.maxDistance = maxDistance;
        audioPassos.rolloffMode = rolloffMode;
      
       

       
    }
    [ServerRpc(RequireOwnership = false)]
    private void ReproduzirRisada_ServerRpc()
    {
        if (IsOwner)
        {
            reproduzirRisada_ClientRpc();
        }
    }
    [ClientRpc]
    private void reproduzirRisada_ClientRpc()
    {
        risadaAudioSource.Play();
        //Debug.Log("rindo");
    }
    [ServerRpc(RequireOwnership = false)]
    private void ReproduzirPassos_ServerRpc()
    {
        if (controller.velocity.magnitude > 0 && !audioPassos.isPlaying)
        {
            reproduzirPassos_ClientRpc();
        }
        else if (controller.velocity.magnitude == 0 && audioPassos.isPlaying)
        {
            pararsompassos_ClientRpc();
        }
    }
    [ClientRpc]
    private void reproduzirPassos_ClientRpc()
    {
        audioPassos.Play();
        //Debug.Log("andando host");
    }
    [ClientRpc]
    private void pararsompassos_ClientRpc()
    {
        audioPassos.Stop();
        //Debug.Log("parando passo");
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
            atacando.OnValueChanged -= ChangersomAtacando;
        }
    }

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        displayName.text = current.ToString();
    }
    void ChangersomAtacando(bool previous, bool current)
    {
       somataquesounce.Play();
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
            ReproduzirPassos_ServerRpc(); // Chame o método para reproduzir os passos
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
            ReproduzirPassos_ServerRpc(); // Chame o método para reproduzir os passos
        }
        else if (IsOwner)
        {
            move = Vector3.zero;
            animations.andando.Value = false;
            audioPassos.Stop(); // Certifique-se de parar o áudio quando parar de andar
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
    [ServerRpc(RequireOwnership = false)]
    private void somataque_ServerRpc()
    {
        somataque_ClientRpc();


    }
    [ClientRpc]
    private void somataque_ClientRpc()
    {
        somataquesounce.Play();
        Debug.Log("tocou o audio");
    }

    public void Atacar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsOwner && GameManager.Instance.timerAtivo.Value)
            {
                animations.atacando.Value = true;
                //audioteste.Instance.playnomaudioclip();
                somataque_ServerRpc();
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
