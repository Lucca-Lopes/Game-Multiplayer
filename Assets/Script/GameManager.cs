using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    //[SerializeField] TimerController timerScript;

    string playerName;

    public NetworkVariable<bool> timerAtivo = new(false);
    public NetworkVariable<int> jogadoresMortos = new(0);
    public NetworkVariable<bool> killerWin = new(false);

    // implementar de novo a interface tutorial e lore 
    public int jogadoresEsperados; 
    public NetworkVariable<ulong> jogadoresProntos = new(1);

    public List<ulong> jogadoresConectados = new();
    Dictionary<ulong, Personagem> sobreviventes = new();
    [SerializeField] private AudioClip musica;
    private AudioSource musicasounce; // Ser� usado para reproduzir o som
    [SerializeField] private AudioClip vento;
    private AudioSource ventosounce;

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
        jogadoresEsperados = PlayerData.numJogadoresLobby;
    }

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            Debug.Log($"GameManager.OnEnable - IsServer {NetworkManager.Singleton.IsServer} Registrando o evento de cliente conectado");
            NetworkManager.Singleton.OnServerStarted += OnServerStartedHandler;

            //Debug.Log("GameManager.OnEnable - Registrando o evento de cliente conectado");
            //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedHandler;

            Debug.Log("GameManager.OnEnable - Registrando o evento de cliente desconectado");
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedHandler;

            timerAtivo.OnValueChanged += ComecarJogo;
        }
        musicasounce = gameObject.AddComponent<AudioSource>();
        musicasounce.clip = musica;
        musicasounce.volume = 0.5f;
        musicasounce.loop = true;
        ventosounce = gameObject.AddComponent<AudioSource>();
        ventosounce.clip = vento;
        ventosounce.loop = true;
        ventosounce.volume = 1.0f;

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


    #region manage de som
    public void ComecarJogo(bool previous, bool current)
    {
        //Debug.Log($"GameManager.ComecarJogo() - {current} jogadores prontos de {jogadoresEsperados} - Ativando timer");
        tocarmusica_ServerRpc();
        tocarvento_ServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void tocarmusica_ServerRpc()
    {
        tocarmusica_ClientRpc();
    }
    [ClientRpc]
    public void tocarmusica_ClientRpc()
    {
        musicasounce.Play();
        //Debug.Log("musica tocando");
    }
    [ServerRpc(RequireOwnership = false)]
    public void tocarvento_ServerRpc()
    {
        tocarvento_ClientRpc();
    }
    [ClientRpc]
    public void tocarvento_ClientRpc()
    {
        ventosounce.Play();
        //Debug.Log("tocando o vento");
    }
    #endregion manage de som


    [ServerRpc(RequireOwnership = false)]
    public void CausarDano_ServerRpc(int quantidade, ulong atacadoId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var sobreviventesAtuais = GameObject.FindGameObjectsWithTag("Sobrevivente");
            foreach (GameObject sobrevivente in sobreviventesAtuais)
            {
                var personagemComponent = sobrevivente.GetComponent<Personagem>();
                if (personagemComponent.OwnerClientId == atacadoId)
                {
                    personagemComponent.vidaJogador.Value -= quantidade;
                    Debug.Log($"Diminuindo a vida do client {personagemComponent.OwnerClientId} para {personagemComponent.vidaJogador.Value}");
                    if (personagemComponent.vidaJogador.Value == 0)
                    {
                        personagemComponent.zzz.Play();
                        ParticulaDormindo_ClientRpc(atacadoId);
                        jogadoresMortos.Value +=1;
                        Debug.Log($"Jogador {atacadoId} entrou em sono profundo");
                    }
                }
            }
            if (jogadoresMortos.Value >= sobreviventesAtuais.Length)
            {
                killerWin.Value = true;
                Instance.timerAtivo.Value = false;
                Debug.Log("Acabou o jogo");
            }
        }
    }

    [ClientRpc]
    public void ParticulaDormindo_ClientRpc(ulong atacadoId)
    {
        var sobreviventesAtuais = GameObject.FindGameObjectsWithTag("Sobrevivente");
        foreach (GameObject sobrevivente in sobreviventesAtuais)
        {
            var personagemComponent = sobrevivente.GetComponent<Personagem>();
            if (personagemComponent.OwnerClientId == atacadoId)
            {
                personagemComponent.zzz.Play();
            }
        }
    }

    #region unused
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

    private void OnClientConnectedHandler(ulong clientId)
    {
        Debug.Log($"OnClientConnectedHandler - Client {clientId} conectado.");
        /*if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"Cliente {clientId} conectado");
            if(clientId == (ulong)jogadoresEsperados && !timerAtivo.Value)
            {
                timerAtivo.Value = true;
            }
        }
        //FetchPlayers();*/
    }

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
    #endregion unused

    public void EncerrarJogo()
    {
        Reinicar restart = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Reinicar>();
        restart.DisconnectPlayers_ServerRpc();
    }

    public void VoltarMenuInicial()
    {
        Debug.Log("VoltarMenuInicial - Clicou em VoltarMenuInicial");
        Time.timeScale = 1;
        Debug.Log("VoltarMenuInicial - Time.timeScale = 1");
        Destroy(GameObject.FindGameObjectWithTag("NetworkManager"));
        Debug.Log("VoltarMenuInicial - Destroy(GameObject.FindGameObjectWithTag(\"NetworkManager\"))");
        SceneManager.LoadScene("MenuLobby", LoadSceneMode.Single);
        Debug.Log("VoltarMenuInicial - SceneManager.LoadScene(\"MenuLobby\", LoadSceneMode.Single)");
    }
}
