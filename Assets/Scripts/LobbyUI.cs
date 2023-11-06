using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyName;
    [SerializeField] TMP_Text totalPlayers;
    [SerializeField] Button apagarBtn;
    [SerializeField] Button entrarBtn;

    public string LobbyId {  get; private set; }

    bool isHost;
    Lobby data;

    public Lobby Data { get { return data; } }

    public string Nome 
    {
        get { return lobbyName.text; }
        private set { lobbyName.text = value; }
    }

    public void InitLobbyData(Lobby lobby, bool _isHost = false, bool _playerNoLobby = false)
    {
        AtualizarDadosSala(lobby);
        isHost = _isHost;

        //Habilita ou não os botões de ação
        PrepararActions(_playerNoLobby);
    }

    public void AtualizarDadosSala(Lobby _data)
    {
        data = _data;
        LobbyId = data.Id;
        TotalPlayers(data.MaxPlayers, data.MaxPlayers - data.AvailableSlots);
        Nome = data.Name;
    }

    private void PrepararActions(bool _playerNoLobby)
    {
        if (isHost)
        {
            entrarBtn.interactable = false;
            ToggleBtn(entrarBtn, false);
            ToggleBtn(apagarBtn, true);
        }
        else
        {
            if (_playerNoLobby)
            {
                ToggleBtn(entrarBtn, false);
            }
            else
            {
                ToggleBtn(entrarBtn, true);
            }
            ToggleBtn(apagarBtn, false);
        }
    }

    public void ApagarLobbyAction()
    {
        MenuGameManager.Lobby.ApagarLobby(LobbyId, MenuGameManager.ApagarLobbyCallback);
    }
    public void EntrarLobbyAction()
    {
        MenuGameManager.Lobby.EntrarLobby(LobbyId, EntrarLobbyUICallback);
    }

    public void TotalPlayers(int maximoLobby, int totalNoLobby)
    {
        totalPlayers.text = string.Concat(totalNoLobby, " / ", maximoLobby);
    }


    public void EntrarLobbyUICallback()
    {
        ToggleBtn(entrarBtn, false);
        ToggleBtn(apagarBtn, false);
        MenuGameManager.SalaController.ShowSalaInfoPanel();
    }

    public void ToggleBtn(Button btn, bool value)
    {
        Debug.Log($"LobbyUI.ToggleBtn - {btn.name}, {value}");
        btn.gameObject.SetActive(value);
    }
}