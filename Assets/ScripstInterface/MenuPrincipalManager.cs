using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJOgo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject paineResolucao;
    [SerializeField] private GameObject painelSom;
    [SerializeField] private GameObject Voltar;
    [SerializeField] private GameObject painelGameOver;
    [SerializeField] private GameObject tutorial1;
    [SerializeField] private GameObject tutorial2;

    public GameObject imageObject1;
    public GameObject imageObject2;
    public GameObject imageObject3;
    public GameObject imageObject4;
    //[SerializeField] private GameObject painelClassificacao;
    [SerializeField] private GameObject painelClassificacao;
    public void Jogar()
    {
        Debug.Log("O jogo foi iniciado");
        //SceneManager.LoadScene("Jogo");
    }
   public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }
   public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }
    public void AbrirVolume()
    {
        painelSom.SetActive(true);
        painelMenuInicial.SetActive(false);
    }
    public void FecharVolume()
    {
        painelSom.SetActive(false);
        painelMenuInicial.SetActive(true);
    }
    public void AbrirResolucao()
    {
        paineResolucao.SetActive(true);
        painelMenuInicial.SetActive(false);
    }
    public void FecharResolucao()
    {
        paineResolucao.SetActive(false);
        painelMenuInicial.SetActive(true);
    }  
   public void SairJOgo()
    {
        Debug.Log("O jogo foi fechado"); 
        Application.Quit();
    }
    public void AbrirClassificacao()
    {
        painelGameOver.SetActive(false);
        painelClassificacao.SetActive(true);
    }
    public void FecharClassificacao()
    {
       
        painelClassificacao.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

  
    // dia 20/10

    public void ContinuaTutorial1()
    {
        tutorial1.SetActive(false);
        tutorial2.SetActive(true);

        imageObject1 = null;
        imageObject2 = null;
        imageObject3 = null;
        imageObject4 = null;

    }
    public void ContinuaTutorial2()
    {
        tutorial2.SetActive(false);
        tutorial1.SetActive(true);
    }
    public void Tutorial()
    {

    }
    
}
