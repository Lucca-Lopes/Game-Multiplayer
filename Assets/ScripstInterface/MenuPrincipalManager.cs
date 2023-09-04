using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField]private string nomeDoLevelDeJOgo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject paineResolucao;
    [SerializeField] private GameObject painelSom;
    [SerializeField] private GameObject Voltar;
    [SerializeField] private GameObject painelGameOver;
    [SerializeField] private GameObject VoltarParaMenu;
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
    public void AbrirResolução()
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
    public void VoltarMenu()
    {
        painelGameOver.SetActive(false);
        painelMenuInicial.SetActive(true);
    }
}
