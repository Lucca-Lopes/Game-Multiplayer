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

    List<ulong> jogadoresConectados = new();
    Dictionary<ulong, Personagem > sobreviventes = new();

    private int jogadoresMortos = 0;

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
            //o número a baixo é sempre um a menos da quantidade de clientes que estão jogando a mesmo tempo, ou seja, se tem 2 crianças, o numero deve ser 1, e se tem 3 crianças, o numero deve ser 2
            if(clientId > 2 && !timerAtivo.Value)         
            {
                timerAtivo.Value = true;
            }
          
            Debug.Log("Deu certo linha 86");
             if(jogadoresMortos == 2)
            {
                killerWin.Value = true;
            }
           
        }
       

    }

    
   // [ServerRpc(RequireOwnership = false)]
    
        

    [ServerRpc(RequireOwnership = false)]
    public void CausarDano_ServerRpc(int quantidade, ulong atacadoId)
    {
       
        if (NetworkManager.Singleton.IsServer)
        {
            var sobreviventesAtuais = GameObject.FindGameObjectsWithTag("Sobrevivente");
           // var inimigoComponent = GameObject.FindGameObjectsWithTag("Inimigo");

           
            foreach (GameObject sobrevivente in sobreviventesAtuais)
            {
                
                var personagemComponent = sobrevivente.GetComponent<Personagem>();
                
                if (personagemComponent.OwnerClientId == atacadoId)
                {
                    if (personagemComponent.vidaJogador.Value > 0)
                    {
                        personagemComponent.vidaJogador.Value--;
                    }
                       

                    if (personagemComponent.isDead)
                    {
                        jogadoresMortos++;
                        Debug.Log("Um jogador morreu");
                        Debug.Log(jogadoresMortos);
                    }

                    /* 
                     if(personagemComponent.vidaJogador.Value <= 0)
                     {
                         Debug.Log("Não é possovel atacar este player");


                     }
                     if (personagemComponent.vidaJogador.Value > 0)
                     {
                         Debug.Log("Este player pode ser atacado");

                     }
                    */
                   
                    if (jogadoresMortos == 2)
                    {
                        Debug.Log("O killer ganhou linha 148");
                        killerWin.Value = true;
                        Instance.timerAtivo.Value = false;
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

            /*
            if (jogadoresMortos == sobreviventesAtuais.Length)
            {
                Debug.Log("O killer ganhou");
                killerWin.Value = true;
                Instance.timerAtivo.Value = false;
            }
            */
        }
    }

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
