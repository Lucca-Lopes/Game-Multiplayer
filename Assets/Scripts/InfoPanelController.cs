using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    [SerializeField] GameObject infoPanelContainer;
    [SerializeField] GameObject infoPanelCriarLobby;
    [SerializeField] GameObject infoPanelPesquisarLobby;

    [Header("Info criar")]
    [SerializeField] Toggle isPrivate;
    [SerializeField] TMPro.TMP_InputField totalJogadores;
    [SerializeField] TMPro.TMP_InputField nomeLobby;
    [SerializeField] TMPro.TMP_InputField tagLobby;

    #region Controles de Tela
    public void ShowInfoPanelPesquisarLobby()
    {
        if(infoPanelCriarLobby.activeSelf)
            HideInfoPanelCriarLobby();

        ShowInfoPanelContainer();
        TogglePanel(infoPanelPesquisarLobby, true);
    }

    public void HideInfoPanelPesquisarLobby()
    {
        TogglePanel(infoPanelPesquisarLobby, false);
        HideInfoPanelContainer();
    }

    public void ShowInfoPanelCriarLobby()
    {
        if (infoPanelPesquisarLobby.activeSelf)
            HideInfoPanelPesquisarLobby();

        ShowInfoPanelContainer();
        TogglePanel(infoPanelCriarLobby, true);
    }

    public void HideInfoPanelCriarLobby()
    {
        TogglePanel(infoPanelCriarLobby, false);
        HideInfoPanelContainer();
    }

    public void HideInfoPanelContainer()
    {
        TogglePanel(infoPanelContainer, false);
    }

    public void ShowInfoPanelContainer()
    {
        TogglePanel(infoPanelContainer, true);
    }

    void TogglePanel(GameObject go, bool value)
    {
        go.SetActive(value);
    }
    #endregion Controles de Tela

    #region Dados de Tela
    public LobbyData GetDadosCriarLobby()
    {
        LobbyData dados = new LobbyData();

        dados.isPrivate = isPrivate.isOn;
        dados.totalJogadores = int.Parse(totalJogadores.text);
        dados.lobbyName = nomeLobby.text;
        dados.tag = tagLobby.text;

        return dados;
    }
    #endregion Dados de Tela
}
