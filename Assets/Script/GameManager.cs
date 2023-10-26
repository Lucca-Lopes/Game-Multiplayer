using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    //[SerializeField] TimerController timerScript;

    string playerName;

    public NetworkVariable<bool> timerAtivo = new(false);
    public NetworkVariable<bool> killerWin = new(false);

    public List<ulong> jogadoresConectados = new();
    Dictionary<ulong, Personagem> sobreviventes = new();

    //private int jogadoresMortos = 0;

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
            //DontDestroyOnLoad(gameObject);
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
    //public override void OnNetworkSpawn()
    //{
    //    timerAtivo.OnValueChanged += StartTimer;
    //}

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
            if(clientId > 2 && !timerAtivo.Value)
            {
                timerAtivo.Value = true;
            }
        }
        //FetchPlayers();
    }

    public void LiberarMovimento()
    {
        GameObject entidade = GameObject.FindGameObjectWithTag("Player");
        entidade.GetComponent<Inimigo>();

        GameObject[] criancas = GameObject.FindGameObjectsWithTag("Sobrevivente");
        foreach (GameObject crianca in criancas)
        {
            var scriptCrianca = crianca.GetComponent<Personagem>();
            Instance.jogadoresConectados.Add(scriptCrianca.OwnerClientId);
            Instance.sobreviventes.Add(scriptCrianca.OwnerClientId, scriptCrianca);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CausarDano_ServerRpc(int quantidade, ulong atacadoId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            int jogadoresMortos = 0;
            var sobreviventesAtuais = GameObject.FindGameObjectsWithTag("Sobrevivente");
            foreach (GameObject sobrevivente in sobreviventesAtuais)
            {
                var personagemComponent = sobrevivente.GetComponent<Personagem>();
                if (personagemComponent.vidaJogador.Value == 0)
                    jogadoresMortos++;
                if (personagemComponent.OwnerClientId == atacadoId)
                {
                    if (personagemComponent.vidaJogador.Value > 0)
                    {
                        personagemComponent.vidaJogador.Value--;
                        jogadoresMortos++;
                    }
                }
                
            }
            if (jogadoresMortos == sobreviventesAtuais.Length)
            {
                killerWin.Value = true;
                Instance.timerAtivo.Value = false;
                Debug.Log("Acabou o jogo");
            }
        }
    }


            //Debug.Log("Recebeu o ataque: " +atacadoId);
            //if (sobreviventes[atacadoId].vidaJogador.Value > 0)
            //{
            //    sobreviventes[atacadoId].vidaJogador.Value -= quantidade;
            //    Debug.Log("Vida sobrevivente: " + sobreviventes[atacadoId].vidaJogador.Value);
            //}
            //if (sobreviventes[atacadoId].isDead)
            //{
            //    jogadoresMortos++;
            //}


            //foreach (ulong clientId in Instance.sobreviventes.Keys)
            //{
            //Debug.Log("KeyAtual: " + clientId);
            //if (clientId == atacadoId)
                //{
                    //Debug.Log("Recebeu o ataque: " + atacadoId + ", encontrado: " +clientId);
                    //if (sobreviventes[clientId].vidaJogador.Value > 0)
                    //{
                    //    sobreviventes[clientId].vidaJogador.Value -= quantidade;
                    //    Debug.Log(sobreviventes[clientId].vidaJogador.Value);
                    //}
                //}
                //if (sobreviventes[clientId].isDead)
                //    jogadoresMortos++;
            //}


            
//    }
//}

//[ServerRpc(RequireOwnership = false)]
//public void SpawnSobrevivente_ServerRpc(ulong clientId)
//{
//    if (NetworkManager.Singleton.IsServer)
//    {
//        sobreviventes[clientId].gameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
//        //sobrevivente.GetComponent<NetworkObject>().Spawn();
//    }
//}

//[ServerRpc(RequireOwnership = false)]
//public void SpawnSobrevivente_ServerRpc()
//{
//    if (NetworkManager.Singleton.IsServer)
//    {
//        var sobreviventes = GameObject.FindGameObjectsWithTag("Sobrevivente");

//        foreach(GameObject sobrevivente in sobreviventes)
//        {
//            sobrevivente.GetComponent<NetworkObject>().Spawn();
//        } 
//    }
//}

    public static void AddPlayer(ulong clientId, Personagem sobrevivente = null)
    {
        Debug.Log(clientId);
        if (!Instance.jogadoresConectados.Contains(clientId))
        {
            Debug.Log(clientId);
            Instance.jogadoresConectados.Add(clientId);
            if(sobrevivente != null)
            {
                if (!Instance.sobreviventes.ContainsKey(clientId))
                {
                    Debug.Log("key novo jogador: " + clientId);
                    Debug.Log("qunatidade sobreviventes: " + Instance.sobreviventes.Count);
                    Instance.sobreviventes.Add(clientId, sobrevivente);
                }
            }
        }
        Debug.Log("Jogadores conectados: " + Instance.jogadoresConectados.Count);
        Debug.Log("Sobreviventes conectados: " + Instance.sobreviventes.Count);
    }

    public static void FetchPlayers()
    {
        //if (Instance.IsOwner)
        //{
            Instance.jogadoresConectados = new();

            GameObject entidade = GameObject.FindGameObjectWithTag("Player");
            Instance.jogadoresConectados.Add(entidade.GetComponent<Inimigo>().OwnerClientId);

            GameObject[] criancas = GameObject.FindGameObjectsWithTag("Sobrevivente");
            foreach (GameObject crianca in criancas)
            {
                var scriptCrianca = crianca.GetComponent<Personagem>();
                Instance.jogadoresConectados.Add(scriptCrianca.OwnerClientId);
                Instance.sobreviventes.Add(scriptCrianca.OwnerClientId, scriptCrianca);
            }

            Debug.Log("Jogadores conectados: " + Instance.jogadoresConectados.Count);
            Debug.Log("Sobreviventes conectados: " + Instance.sobreviventes.Count);
        //}
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
