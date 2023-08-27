using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Personagem : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 movimento;
    private InputAction interactAction;

    public GameObject objetoInterativo;
    public float distanciaMaxima = 3.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        interactAction.performed += Interact;
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
    }
    private void OnEnable()
    {
        interactAction.Enable();
    }
    private void OnDisable()
{
    interactAction.Disable();
}
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float distancia = Vector3.Distance(transform.position, objetoInterativo.transform.position);
            if (distancia <= distanciaMaxima)
            {
                Debug.Log("Objeto detectado! Realize a intera��o.");
                // Execute sua l�gica de intera��o aqui
            }
            else
            {
                Debug.Log("Voc� est� muito longe para interagir com o objeto.");
            }
        }
    }

    /*
    public void SetPular(InputAction.CallbackContext value)
    {
        rb.AddForce(Vector3.up * 100);
    }
    */
    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(movimento.x, 0, movimento.y) * Time.fixedDeltaTime * 300);
    }
}
