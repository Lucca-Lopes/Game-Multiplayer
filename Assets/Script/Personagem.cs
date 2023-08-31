using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Personagem : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 movimento;
    private InputAction interactAction;

    public ProgressionBar progressionBar;

    private bool isInteracting = false;

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
            if(!isInteracting)
            { 

            float distancia = Vector3.Distance(transform.position, objetoInterativo.transform.position);
            if (distancia <= distanciaMaxima)
            {
                Debug.Log("Iniciando Interação...");
                    isInteracting=true;
                    StartCoroutine(InteractWithProgressBar());
                    // Execute sua lógica de interação aqui
                }
            else
            {
                Debug.Log("Você está muito longe para interagir com o objeto.");
            }
            
           }
        }
    }
    private IEnumerator InteractWithProgressBar()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < progressionBar.duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // A interação foi concluída após a barra de progressão preencher completamente
        Debug.Log("Interagiu com o objeto!");
        isInteracting = false;
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
