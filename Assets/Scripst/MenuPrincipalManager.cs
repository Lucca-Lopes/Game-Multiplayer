using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField]private string nomeDoLevelDeJOgo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
   public void Jogar()
    {
        SceneManager.LoadScene("Jogo");
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
   public void SairJOgo()
    {
        Debug.Log("O jogo foi fechado"); 
        Application.Quit();
    }
}
