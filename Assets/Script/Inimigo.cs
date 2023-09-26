using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
//using static UnityEngine.Rendering.DebugUI;

public class Inimigo : NetworkBehaviour
{
    public float distanciaAtaque = 2.0f;
    public int dano = 1;
    public int velocidade = 600;
    private Vector2 movimento;
    private Rigidbody rb;
    public float distanciaCarregamento = 2.0f;
    [SerializeField] private CinemachineFreeLook vc;
    //[SerializeField] EfeitoVisual efeitoScript;

    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] TMPro.TextMeshProUGUI displayName;

    [SerializeField] ParticleSystem efeito;

    private void Awake()
    {
        //interactaction = new InputAction("Interact", binding: "<KeyBoard>/Space");
        //interactaction.performed += CarregarJogador;
        rb = GetComponent<Rigidbody>();
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

    //public void CarregarJogador(InputAction.CallbackContext value)
    //{
    //    if (jogador.vidas <= 0 && value.started)
    //    {
    //        float distanciaJogadorInimigo = Vector3.Distance(transform.position, jogador.transform.position);
    //        if (distanciaJogadorInimigo <= distanciaCarregamento) 
    //        {
    //            Personagem player = FindObjectOfType<Personagem>();
    //            if (player != null)
    //            {
    //                player.SerCarregadoPorInimigo(this);
    //                velocidade = 800;
    //            }
    //        }
    //    }
    //    else if (value.canceled && jogador.isBeingCarried)
    //    {
    //        Personagem player = FindObjectOfType<Personagem>();
    //        if (player != null)
    //        {
    //            player.transform.SetParent(jogador.previousParent);
    //            jogador.isBeingCarried = false;
    //            velocidade = 600;


    //            Vector3 offset = transform.forward * 2.0f; 
    //            player.transform.position = transform.position + offset;

    //            player.velocidade = 350;
    //            //player.GetComponent<Rigidbody>().isKinematic = false;
    //        }
    //    }
    //}

    private void FixedUpdate()
    {
 
        if (rb != null)
        {
            rb.AddForce(new Vector3(movimento.x, 0, movimento.y) * Time.fixedDeltaTime * velocidade);
        }
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
}
