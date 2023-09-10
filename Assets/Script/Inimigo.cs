using UnityEngine.InputSystem;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    public float distanciaAtaque = 2.0f;
    public int dano = 1;
    public int velocidade = 600;
    private Vector2 movimento;
    private Rigidbody rb;

    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();

        // Certifique-se de que o objeto tenha um componente Rigidbody.
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("O objeto não possui um componente Rigidbody.");
        }
    }

    private void FixedUpdate()
    {
        // Verifique se rb não é nulo antes de chamar AddForce.
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
