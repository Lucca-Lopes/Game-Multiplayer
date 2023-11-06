using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    Lobby lobbyAtual;

    public Lobby MeuLobby
    {
        get
        {
            return lobbyAtual;
        }
    }

    public async Task<List<Lobby>> ListarLobby()
    {
        List<Lobby> list = new List<Lobby>();
        try
        {
            QueryResponse query = await LobbyService.Instance.QueryLobbiesAsync();
            if (query != null && query.Results.Count > 0)
            {
                list = query.Results;
                //Atualizando a lista
                if(lobbyAtual != null)
                {
                    AtualizarLobby(list);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return list;
    }

    public async void CriarLobby(LobbyData lobbyData, Action<Lobby> callback = null)
    {
        if (lobbyData == null)
        {
            Debug.LogError("Algo de errado aconteceu, não vejo as informações dolobby!");
        }
        //Criando opções do lobby
        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
        lobbyOptions.Data = new Dictionary<string, DataObject>();
        //adicionando o hostCode vazio
        lobbyOptions.Data.Add("hostCode", new DataObject(DataObject.VisibilityOptions.Public, string.Empty));
        //Verificando se chegou alguma tag
        if (!string.IsNullOrEmpty(lobbyData.tag))
        {
            //Criando o dataObject para a tag
            DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, lobbyData.tag, DataObject.IndexOptions.S1);
            //adicionando a tag no lobby.Data
            lobbyOptions.Data.Add("LobbyTag", dataObject);
        }
        //Adicionando dados do Jogador
        lobbyOptions.Player = MenuGameManager.Jogador;
        try
        {
            lobbyAtual = await LobbyService.Instance.CreateLobbyAsync(lobbyData.lobbyName, lobbyData.totalJogadores, lobbyOptions);
            callback?.Invoke(lobbyAtual);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void EntrarLobby(string lobbyId, Action callback = null)
    {
        try
        {
            JoinLobbyByIdOptions options = new();
            options.Player = MenuGameManager.Jogador;
            lobbyAtual = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
            callback?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void AtualizarDadosLobby(KeyValuePair<string, string> entry)
    {
        UpdateLobbyOptions options = new UpdateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>();
        options.Data.Add(entry.Key, new DataObject(DataObject.VisibilityOptions.Public, entry.Value));
        try
        {
            lobbyAtual = await LobbyService.Instance.UpdateLobbyAsync(lobbyAtual.Id, options);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void AtualizarLobby(List<Lobby> lista)
    {
        foreach (Lobby lobby in lista)
        {
            if (lobby.Id.Equals(lobbyAtual.Id))
            {
                lobbyAtual = lobby;
                break;//Finaliza o loop
            }
        }
    }

    public async void AtualizarJogadorPronto(string playerId, bool pronto)
    {
        UpdatePlayerOptions options = new UpdatePlayerOptions();
        options.Data = MenuGameManager.Jogador.Data;
        if (!options.Data.ContainsKey("pronto"))
            options.Data.Add("pronto", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, pronto.ToString()));
        else
            options.Data["pronto"].Value = pronto.ToString();
        try
        {
            lobbyAtual = await LobbyService.Instance.UpdatePlayerAsync(lobbyAtual.Id, playerId, options);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void SairLobby(string playerId)
    {
        try
        {
            if (lobbyAtual.HostId.Equals(playerId))
                MenuGameManager.PararHeartbeat(lobbyAtual.Id);
            await LobbyService.Instance.RemovePlayerAsync(lobbyAtual.Id, playerId);
            lobbyAtual = null;
            await ListarLobby();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void ApagarLobby(string lobbyId, Action<string> callback = null)
    {
        try
        {
            if (lobbyAtual.Id.Equals(lobbyId))
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
                lobbyAtual = null;
                callback?.Invoke(lobbyId);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public IEnumerator Heartbeat(string lobbyId, float interval)
    {
        Debug.Log($"LobbyManager.Heartbeat: {lobbyId}, por {interval} segundos");
        while (true)
        {
            Debug.Log($"Ping no lobby {lobbyId}");
            //Chamando o ping
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            //Esperando o tempo
            yield return new WaitForSeconds(interval);
        }
    }
}