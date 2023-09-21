using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;

public class Inimigo : NetworkBehaviour
{
    public float distanciaAtaque = 2.0f;
    public int dano = 1;
    public int velocidade = 600;
    private Vector2 movimento;
    private InputAction interactaction;
    private Rigidbody rb;
    private Personagem jogador; 
    public float distanciaCarregamento = 2.0f;


    private void Awake()
    {
        jogador = FindObjectOfType<Personagem>();
        if (jogador == null)
        {
            Debug.LogError("N�o foi poss�vel encontrar o jogador.");
        }
        interactaction = new InputAction("Interact", binding: "<KeyBoard>/Space");
        interactaction.performed += CarregarJogador;
        rb = GetComponent<Rigidbody>();
    }
    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
        
       
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
                }
            }
        }
        else if (value.canceled && jogador.isBeingCarried)
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
                //player.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }







    private void FixedUpdate()
    {
 
        if (rb != null)
        {
            rb.AddForce(new Vector3(movimento.x, 0, movimento.y) * Time.fixedDeltaTime * velocidade);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, distanciaAtaque);
            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player"))
                {
                    //col.GetComponent<Personagem>().ReceberDano(dano);
                    GameManager.Instance.CausarDano_ServerRpc(1, col.GetComponent<Personagem>().OwnerClientId);
                }
            }
        }
    }
}
