using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetworkManagerUI : NetworkBehaviour
{
    //[SerializeField] private Button serverButton;
    //[SerializeField] private Button hostButton;
    //[SerializeField] private Button clientButton;

    [Header("Configurações Input de Nome")]
    [SerializeField] GameObject errorTextInput;
    [SerializeField] TMP_InputField playerName;
    [SerializeField] GameObject interfacePlayerName;

    [Header("Configurações Lobby")]
    [SerializeField] GameObject painelLobby;
    [SerializeField] GameObject errorTextLobby;
    [SerializeField] TextMeshProUGUI[] nomesJogadores;
    [SerializeField] GameObject[] checkmarksJogadores;
    [SerializeField] GameObject botaoComecar;

    [Header("NetVar Players prontos")]
    public NetworkVariable<bool> playerPronto = new(false);
    public NetworkVariable<int> indexNomeAtual = new(0);

    private void Awake()
    {
        playerPronto.OnValueChanged += AtualizarCheckmarks;
        NetworkManager.OnClientConnectedCallback += ClientConnectedHandler;
    }

    //private void Awake()
    //{
    //    serverButton.onClick.AddListener(() => { 
    //        NetworkManager.Singleton.StartServer();
    //    } );
    //    hostButton.onClick.AddListener(() => {
    //        NetworkManager.Singleton.StartHost();
    //    });
    //    clientButton.onClick.AddListener(() => {
    //        NetworkManager.Singleton.StartClient();
    //    });
    //}

    public void StartHostHandler()
    {
        if (!IsInputFieldEmpty(playerName))
        {
            GameManager.PlayerName = playerName.text;
            NetworkManager.Singleton.StartHost();
            interfacePlayerName.SetActive(false);
            painelLobby.SetActive(true);
            botaoComecar.SetActive(true);
            nomesJogadores[indexNomeAtual.Value].text = GameManager.PlayerName;
            UpdateIndexNome_ServerRpc();
        }
        else
            errorTextInput.SetActive(true);
    }

    public void StartClientHandler()
    {
        if (!IsInputFieldEmpty(playerName))
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) {
                GameManager.PlayerName = playerName.text;
                NetworkManager.Singleton.StartClient();
                interfacePlayerName.SetActive(false);
                painelLobby.SetActive(true);
                botaoComecar.SetActive(false);
                nomesJogadores[indexNomeAtual.Value].text = GameManager.PlayerName;
                UpdateIndexNome_ServerRpc();
            }
        }
        else
            errorTextInput.SetActive(true);
    }

    private void ClientConnectedHandler(ulong clientId)
    {

    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGame_ServerRpc()
    {
        if (PlayersProntos())
        {
            var spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
            spawnManager.GetComponent<SpawnManager>().SpawnPlayers();
        }
    }

    private bool PlayersProntos()
    {
        int playerProntos = 0;
        foreach (GameObject playerCheckmark in checkmarksJogadores)
        {
            if (playerCheckmark.activeInHierarchy)
            {
                playerProntos++;
            }
        }
        if (playerProntos == checkmarksJogadores.Length)
            return true;
        else
            return false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReady_ServerRpc()
    {
        playerPronto.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateIndexNome_ServerRpc()
    {
        indexNomeAtual.Value++;
    }

    private void AtualizarCheckmarks(bool previous, bool current)
    {
        checkmarksJogadores[OwnerClientId].SetActive(current);
    }

    bool IsStringEmpty(string value)
    {
        return string.IsNullOrEmpty(value);
    }

    bool IsInputFieldEmpty(TMP_InputField inputField)
    {
        return IsStringEmpty(inputField.text);
    }
}
