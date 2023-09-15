using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaPlayer : MonoBehaviour
{
    public int vida = 10;      
    void Update()
    {
        Morte(); 
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstaculo"))
        {
            Debug.Log("Perdi Vida");
            vida--;
        }
    }
    public void Morte()
    {
        if(vida <= 0)
        {
            Debug.Log("Morri");
            Destroy(gameObject);
        }
    }
}
