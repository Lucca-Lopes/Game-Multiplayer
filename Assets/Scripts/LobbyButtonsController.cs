using UnityEngine;

[RequireComponent (typeof(InfoPanelController))]
public class LobbyButtonsController : MonoBehaviour
{
    [SerializeField] InfoPanelController panelController;
    [SerializeField] TMPro.TMP_InputField nomeLobby;
    [SerializeField] TMPro.TMP_InputField totalJogadores;
    [SerializeField] GameObject erroNomeText;
    [SerializeField] GameObject erroNumText;
    [SerializeField] GameObject salaLobbyPanel;
    [SerializeField] GameObject criarLobbyPanel;

    private void Start()
    {
        panelController = GetComponent<InfoPanelController>();
    }

    public void CriarLobbyBtnHandler()
    {
        panelController.ShowInfoPanelCriarLobby();
    }

    public void PesquisarLobbyBtnHandler()
    {
        panelController.ShowInfoPanelPesquisarLobby();
    }

    public void CriarLobbyAction()
    {
        bool nomeCerto = true;
        bool numJogadoresCerto = true;
        int numJogadores;
        erroNomeText.SetActive(false);
        erroNumText.SetActive(false);
        if (string.IsNullOrEmpty(nomeLobby.text))
        {
            erroNomeText.SetActive(true);
            nomeCerto = false;
        }
        if (string.IsNullOrEmpty(totalJogadores.text))
        {
            erroNumText.SetActive(true);
            numJogadoresCerto = false;
        }
        if(nomeCerto)
        {
            erroNomeText.SetActive(false);
            if (numJogadoresCerto)
            {
                numJogadores = int.Parse(totalJogadores.text);
                if (numJogadores >= 2 && numJogadores <= 6)
                {
                    LobbyData lobbyData = panelController.GetDadosCriarLobby();
                    MenuGameManager.Lobby.CriarLobby(lobbyData, MenuGameManager.CriarLobbyCallback);
                    salaLobbyPanel.SetActive(true);
                    erroNomeText.SetActive(false);
                    erroNumText.SetActive(false);
                    criarLobbyPanel.SetActive(false);
                }
                else
                {
                    erroNumText.SetActive(true);
                }
            }
            else
            {
                erroNumText.SetActive(true);
            }
        }
        if (numJogadoresCerto)
        {
            numJogadores = int.Parse(totalJogadores.text);
            if (numJogadores < 2 || numJogadores > 6)
            {
                erroNumText.SetActive(true);
            }
        }
    }

    public void PesquisarLobbyAction()
    {
        
        Debug.Log($"Pesquisando o lobby");
    }

    public void ExcluirLobbyAction()
    {

    }
}
