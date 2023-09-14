using UnityEngine.InputSystem;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    public float distanciaAtaque = 2.0f;
    public int dano = 1;
    public int velocidade = 600;
    private Vector2 movimento;
    private InputAction interactaction;
    private Rigidbody rb;
    private Personagem jogador; // Adicione isso como uma variável de membro na classe Inimigo.



    private void Awake()
    {
        jogador = FindObjectOfType<Personagem>();
        if (jogador == null)
        {
            Debug.LogError("Não foi possível encontrar o jogador.");
        }
        interactaction = new InputAction("Interact", binding: "<KeyBoard>/Space");
        interactaction.performed += setinterajir;
        rb = GetComponent<Rigidbody>();
    }
    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
        
       
    }
    public void setinterajir(InputAction.CallbackContext value)
    {
        if (jogador.vidas <= 0 && value.started)
        {
            Personagem player = FindObjectOfType<Personagem>();
            if (player != null)
            {
                player.SerCarregadoPorInimigo(this);
                velocidade = 600;
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
               
                player.velocidade = 350;
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
                    col.GetComponent<Personagem>().ReceberDano(dano);
                }
            }
        }
    }
}
