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
    [SerializeField] private GameObject salaLobby;
    [SerializeField] TMPro.TMP_InputField nomeJogador;
    [SerializeField] TMPro.TextMeshProUGUI feedbackLobby;
    [SerializeField] GameObject erroNomeJogador;
    [SerializeField] private GameObject lobby;
    [SerializeField] private AudioSource audioSource; // Adicione essa vari�vel para referenciar o AudioSource

    [SerializeField] private AudioClip seuAudioClip; // Refer�ncia para o �udio que voc� importou

    [Header("Lore/Tutorial/Pr�-Jogo")]
    [SerializeField] GameObject interfaceLore;
    [SerializeField] GameObject entidadeTutorial;
    [SerializeField] GameObject criancaTutorial;


    private void Update()
    {
        if (MenuGameManager.Lobby.MeuLobby != null && MenuGameManager.isConnectedLobby)
        {
            salaLobby.SetActive(true);
            lobby.SetActive(false);
            feedbackLobby.text = "Carregando sala...";
        }
        else if(MenuGameManager.Lobby.MeuLobby == null && MenuGameManager.isConnectedLobby)
        {
            feedbackLobby.text = "Erro ao encontrar sala.";
        }
    }

    public void Jogar()
    {
        inputNomeJogar.SetActive(true);
        painelMenuInicial.SetActive(false);
    }

    public void MostrarLobbies()
    {
        if (string.IsNullOrEmpty(nomeJogador.text))
        {
            erroNomeJogador.SetActive(true);
        }
        else
        {
            MenuGameManager.CriarDadosJogador();
            lobby.SetActive(true);
            erroNomeJogador.SetActive(false);
            inputNomeJogar.SetActive(false);
        }
    }

    //public void AbrirSala()
    //{
    //salaLobby.SetActive(true);
    //    lobby.SetActive(false);
    //}

    public void TocarSom()
    {
        if (audioSource && seuAudioClip) // Verifica se o AudioSource e o AudioClip est�o configurados
        {
            audioSource.PlayOneShot(seuAudioClip); // Toca o �udio
        }
    }
    public void FecharJogar()
    {
        painelMenuInicial.SetActive(true);
        lobby.SetActive(false);
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

    public void AbrirLore()
    {
        interfaceLore.SetActive(true);
        painelMenuInicial.SetActive(false);
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

    //    Lore/Tutorial/Pr�-Jogo
    public void TutorialScreen()
    {
        criancaTutorial.SetActive(true);
        interfaceLore.SetActive(false);
    }

    public void TutorialEntidade()
    {
        entidadeTutorial.SetActive(true);
        criancaTutorial.SetActive(false);
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

    public void SairSalaAction()
    {
        if(MenuGameManager.Lobby.MeuLobby != null)
        {
            if(MenuGameManager.Lobby.MeuLobby.HostId.Equals(MenuGameManager.PlayerId)) {
                MenuGameManager.Lobby.ApagarLobby(MenuGameManager.Lobby.MeuLobby.Id, MenuGameManager.ApagarLobbyCallback);
            }
            else
            {
                MenuGameManager.Lobby.SairLobby(MenuGameManager.PlayerId);
            }
            MenuGameManager.isConnectedLobby = false;
            lobby.SetActive(true);
            salaLobby.SetActive(false);
        }
    }
}
