using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJOgo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelConfiguracao;
    [SerializeField] private GameObject paineResolucao;
    [SerializeField] private GameObject painelSom;
    [SerializeField] private GameObject Voltar;
    [SerializeField] private GameObject painelGameOver;
    [SerializeField] private GameObject painelMovimentacao;
    [SerializeField] private GameObject painelObjetivo;
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
    public void AbrirConfiguracao()
    {
        painelMenuInicial.SetActive(false);
        painelConfiguracao.SetActive(true);
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
    public void Tutoriel()
    {
        SceneManager.LoadScene("Tutoriel");
    }
    public void CenaMenu()
    {
        SceneManager.LoadScene("RenatoInterface");
    }
    public void AbreObjetivo()
    {
        painelMovimentacao.SetActive(false);
        painelObjetivo.SetActive(true);
    }
    public void AbreMovimentacao()
    {
        painelObjetivo.SetActive(false);
        painelMovimentacao.SetActive(true);
    }
}
