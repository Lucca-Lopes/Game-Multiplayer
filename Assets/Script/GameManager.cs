using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    string playerName;

    Dictionary<ulong, Personagem> players = new();

    public static string PlayerName
    {
        get { return Instance.playerName; }
        set
        {
            Instance.playerName = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            Debug.Log($"GameManager.OnEnable - IsServer {NetworkManager.Singleton.IsServer} Registrando o evento de cliente conectado");
            NetworkManager.Singleton.OnServerStarted += OnServerStartedHandler;

            Debug.Log("GameManager.OnEnable - Registrando o evento de cliente conectado");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedHandler;

            Debug.Log("GameManager.OnEnable - Registrando o evento de cliente desconectado");
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedHandler;
        }
    }

    private void OnClientDisconnectedHandler(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"Cliente {clientId} Desconectado");
        }
    }

    private void OnServerStartedHandler()
    {
        Debug.Log("Host conectado");
    }

    private void OnClientConnectedHandler(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"Cliente {clientId} conectado");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CausarDano_ServerRpc(int quantidade, ulong atacadoId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            foreach (ulong clientId in players.Keys)
            {
                if (clientId == atacadoId)
                {
                    if (players[clientId].vidaJogador.Value > 0)
                    {
                        players[clientId].vidaJogador.Value--;
                    }
                }
            }
        }
    }
}
