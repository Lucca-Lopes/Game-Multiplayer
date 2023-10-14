using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    public GameObject objetoParaAtivar;
    public Transform player;
    [SerializeField] Personagem scriptPersonagem;
    public Transform player2;
    public float distanciaMinimaParaAtivar = 10.0f;
    private bool objetoAtivado = false;
    void Update()
    {
        Detecta();       
    }
    private void OnDrawGizmos()
    {       
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaMinimaParaAtivar);
    }
    public void Detecta()
    {
        float distanciaAoPlayer = Vector3.Distance(transform.position, player.position);
        if (distanciaAoPlayer <= distanciaMinimaParaAtivar && !objetoAtivado)
        {          
            objetoParaAtivar.SetActive(true);
            objetoAtivado = true;
        }
    }

}
