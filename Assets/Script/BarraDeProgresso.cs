using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BarraDeProgresso : MonoBehaviour
{
    public GameObject Player;
    [SerializeField] GameObject textoBarraMorte;
    //private float Y = 0.1267033f;
    private float X = 10;
    public int maxProg;
    public int currentProg;
    public float taxaDeDiminuicao = 0.1f;
    private float valorAtual;
    private void Start()
    {
        valorAtual = currentProg;
        textoBarraMorte.SetActive(true);
    }
    private void Update()
    {    
        Diminui();
        parar();
        MataPlayer();
        transform.localScale = new Vector3(valorAtual * X/ maxProg, transform.localScale.y, 1);              
    }
    public void Diminui()
    {      
        if (valorAtual >0)
        {
            valorAtual -= taxaDeDiminuicao * Time.deltaTime;
            valorAtual = Mathf.Max(valorAtual, 0);
            Debug.Log("Valor Atual: " + valorAtual);
           
        }
    }   
    public void parar()
    {
        if(valorAtual <= 0)
        {
            taxaDeDiminuicao = 0;
        }
    }
    public void MataPlayer()
    {
        if(valorAtual <= 0)
        {
            Destroy(Player);
            textoBarraMorte.SetActive(false);
            Debug.Log("Voc� Morreu");
           
        }
    }
}