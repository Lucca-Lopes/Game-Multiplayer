using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class SalaPanelController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI nomeSala;
    [SerializeField] Transform jogadorContainer;
    [SerializeField] JogadorInfoLobby jogadorPrefab;
    [SerializeField] Button jogarBtn;

    Dictionary<string, JogadorInfoLobby> listaJogadores = new();
    string salaPrefix = "Sala: ";

    public void ShowSalaInfoPanel()
    {
        Debug.Log($"SalaPanelController.ShowSalaInfoPanel");
        gameObject.SetActive(true);
        IniciarSala();
    }

    public void HideSalaInfoPanel()
    {
        gameObject.SetActive(false);
    }

    void IniciarSala()
    {
        Debug.Log($"SalaPanelController.IniciarSala - Iniciando");
        AtualizarNomeSala(MenuGameManager.Lobby.MeuLobby.Name);
        AtualizarListaJogadores(MenuGameManager.Lobby.MeuLobby.Players);
    }

    public void AtualizarNomeSala(string nomeSala)
    {
        Debug.Log($"SalaPanelController.AtualizarNomeSala - Atualizando o nome para {nomeSala}");
        this.nomeSala.text = string.Concat(salaPrefix, nomeSala);
    }

    public void AtualizarListaJogadores(List<Player> players)
    {
        CriarListaJogadores(players);
        //Removendo os que não estão mais na lista
        RemoverJogadores(players);
    }

    void CriarListaJogadores(List<Player> players)
    {
        int totalJogadoresProntos = 0;
        if (listaJogadores == null)
            listaJogadores = new Dictionary<string, JogadorInfoLobby>();
        foreach (Player p in players)
        {
            if (listaJogadores.ContainsKey(p.Id))
            {
                AtualizarJogadorUI(p, JogadorLocal(p));
            }
            else
            {
                CriarJogador(p);
            }
            if (p.Data.TryGetValue("pronto", out PlayerDataObject obj))
            {
                totalJogadoresProntos = (bool.Parse(obj.Value) ? totalJogadoresProntos + 1 : totalJogadoresProntos);
            }
        }
        bool ativarBtnJogar = totalJogadoresProntos == players.Count && totalJogadoresProntos == MenuGameManager.Lobby.MeuLobby.MaxPlayers && MenuGameManager.Lobby.MeuLobby.HostId.Equals(MenuGameManager.PlayerId);
        jogarBtn.gameObject.SetActive(ativarBtnJogar);
    }

    void RemoverJogadores(List<Player> players)
    {
        IEnumerable<string> paraExlcuir = listaJogadores.Keys.Except(players.Select(p => p.Id)).ToList();
        foreach (string item in paraExlcuir)
        {
            Destroy(listaJogadores[item].gameObject);
            listaJogadores.Remove(item);
        }
    }

    public void CriarJogador(Player player)
    {
        listaJogadores.Add(player.Id, Instantiate(jogadorPrefab, jogadorContainer));
        AtualizarJogadorUI(player, JogadorLocal(player));
    }

    public void AtualizarJogadorUI(Player jogador, bool isLocal)
    {
        listaJogadores[jogador.Id].IniciarJogador(jogador, isLocal);
    }

    bool JogadorLocal(Player player)
    {
        return MenuGameManager.Jogador.Id.Equals(player.Id);
    }

    public void JogarAction()
    {
        //Verificar se o jogador que clicou é o host
        if (!MenuGameManager.Lobby.MeuLobby.HostId.Equals(MenuGameManager.PlayerId))
        {
            Debug.LogError($"SalaPanelContorller.JogarAction - Somente o Host possui permissão para iniciar a partida!");
            return;
        }
        MenuGameManager.IniciarJogo();
    }

    public void LimparSala()
    {
        for (int i = 0; i < jogadorContainer.childCount; i++)
        {
            Destroy(jogadorContainer.GetChild(i).gameObject);
        }
        listaJogadores.Clear();
    }
}
