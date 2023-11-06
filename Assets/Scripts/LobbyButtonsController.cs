using UnityEngine;

[RequireComponent (typeof(InfoPanelController))]
public class LobbyButtonsController : MonoBehaviour
{
    [SerializeField] InfoPanelController panelController;

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
        LobbyData lobbyData = panelController.GetDadosCriarLobby();
        MenuGameManager.Lobby.CriarLobby(lobbyData, MenuGameManager.CriarLobbyCallback);
    }

    public void PesquisarLobbyAction()
    {
        
        Debug.Log($"Pesquisando o lobby");
    }

    public void ExcluirLobbyAction()
    {

    }
}
