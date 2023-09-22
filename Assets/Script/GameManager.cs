using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    string playerName;

    List<ulong> jogadoresConectados = new();
    Dictionary<ulong, Personagem> sobreviventes = new();

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
            foreach (ulong clientId in sobreviventes.Keys)
            {
                if (clientId == atacadoId)
                {
                    if (sobreviventes[clientId].vidaJogador.Value > 0)
                    {
                        sobreviventes[clientId].vidaJogador.Value--;
                    }
                }
            }
        }
    }

    public static void AddPlayer(ulong clientId, Personagem sobrevivente = null)
    {
        if (Instance.jogadoresConectados != null && !Instance.jogadoresConectados.Contains(clientId))
        {
            Instance.jogadoresConectados.Add(clientId);
            if(sobrevivente != null)
            {
                if (!Instance.sobreviventes.ContainsKey(clientId))
                {
                    Instance.sobreviventes.Add(clientId, sobrevivente);
                }
            }
        }
    }

    public static void RemovePlayer(ulong clientId)
    {
        if (Instance.jogadoresConectados != null && Instance.jogadoresConectados.Contains(clientId))
        {
            Instance.jogadoresConectados.Remove(clientId);

            if (Instance.sobreviventes.ContainsKey(clientId))
            {
                Instance.sobreviventes.Remove(clientId);
            }
        }
    }
}
