using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : NetworkBehaviour
{
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
    [SerializeField] private GameObject painelTutorial;
    [SerializeField] private GameObject inputNomeJogar;
    [SerializeField] private AudioSource audioSource; // Adicione essa variável para referenciar o AudioSource

    [SerializeField] private AudioClip seuAudioClip; // Referência para o áudio que você importou

    [Header("Lore/Tutorial/Pré-Jogo")]
    [SerializeField] GameObject interfaceLore;
    [SerializeField] GameObject entidadeTutorial;
    [SerializeField] GameObject criancaTutorial;

    public void Jogar()
    {
        inputNomeJogar.SetActive(true);
        painelMenuInicial.SetActive(false);
    }
    public void TocarSom()
    {
        if (audioSource && seuAudioClip) // Verifica se o AudioSource e o AudioClip estão configurados
        {
            audioSource.PlayOneShot(seuAudioClip); // Toca o áudio
        }
    }
    public void FecharJogar()
    {
        painelMenuInicial.SetActive(true);
        inputNomeJogar.SetActive(false);
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
    public void AbrirTutorial()
    {
        painelTutorial.SetActive(true);
        AbreMovimentacao();
        painelMenuInicial.SetActive(false);
        
    }

    public void FecharTutorial()
    {
        painelTutorial.SetActive(false);
        painelMenuInicial.SetActive(true);

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

    //    Lore/Tutorial/Pré-Jogo
    public void TutorialScreen()
    {
        if (IsHost)
            entidadeTutorial.SetActive(true);
        else
            criancaTutorial.SetActive(true);
        interfaceLore.SetActive(false);
    }
    public void CloseInterface()
    {
        if (IsHost)
            entidadeTutorial.SetActive(false);
        else
            criancaTutorial.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IncreaseReadyCount_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseReadyCount_ServerRpc()
    {
        GameManager.Instance.jogadoresProntos.Value += 1;
        Debug.Log($"MenuPrincipalManager.IncreaseReadyCount() - {GameManager.Instance.jogadoresProntos.Value} players prontos");
    }
}
