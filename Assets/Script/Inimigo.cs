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
    private Vector2 mouseInput;
    private Rigidbody rb;
    public float distanciaCarregamento = 2.0f;
    public Personagem outroJogador; 
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
    private void Update()
    {
        //ParaAtaque();
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
            // Calcular a direção com base na rotação atual
            Vector3 moveDirection = Quaternion.Euler(0, vc.State.CorrectedOrientation.eulerAngles.y, 0) * new Vector3(movimento.x, 0, movimento.y);

            // Aplicar uma força na direção calculada
            rb.AddForce(moveDirection.normalized * Time.fixedDeltaTime * velocidade);
        }
        RotateWithMouseInput();
       // ParaAtaque();
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
;             
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
    public void ParaAtaque()
    {
        // Verificar se o outro jogador está disponível e se a vida dele é maior que 0
        if (outroJogador.vidaPlayer > 0)
        {
            Debug.Log("Posso dar dano");
            // Se a vida do outro jogador for maior que 0, definir meuAtaque para 1
            dano = 1;
        }
      
       
        if(outroJogador.vidaPlayer <= 0)
        {
            Debug.Log("Não posso dar dano linha 180");
            // Se a vida do outro jogador for 0 ou menos, definir meuAtaque para 0
            dano = 0;
        }
    }
}
