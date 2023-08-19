using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Personagem : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 movimento; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetMovimento(InputAction.CallbackContext value)
    {
        movimento = value.ReadValue<Vector2>();
    }
    public void SetPular(InputAction.CallbackContext value)
    {
        rb.AddForce(Vector3.up * 100);
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(movimento.x, 0, movimento.y) * Time.fixedDeltaTime * 300);
    }
}
