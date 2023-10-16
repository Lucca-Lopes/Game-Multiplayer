using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using Cinemachine;

public class PlayerUIManager : NetworkBehaviour
{
    [Header("Configurações Lobby")]
    [SerializeField] GameObject painelLobby;
    [SerializeField] GameObject errorTextLobby;
    [SerializeField] TextMeshProUGUI[] nomesJogadores;
    [SerializeField] GameObject[] checkmarksJogadores;
    [SerializeField] GameObject botaoComecar;
    private Coroutine heartbeat;

    [Header("NetVar Players prontos")]
    public NetworkVariable<bool> playerPronto = new(false);
    public NetworkVariable<FixedString32Bytes> nomeJogador = new(string.Empty, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Variável para o Nome
    [SerializeField] TextMeshProUGUI displayName;
    //Variável para controle de câmera
    [SerializeField] private CinemachineFreeLook vc;


    private void Start()
    {
        if (IsOwner)
        {
            nomeJogador.Value = GameManager.PlayerName;
            heartbeat = StartCoroutine(CheckmarksHeartBeat());
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner)
        {
            nomeJogador.OnValueChanged += OnPlayerNameChanged;
            nomeJogador.Value = GameManager.PlayerName;
            AtualizarNomesDisplay();
        }
        if (IsHost && IsOwner)
            botaoComecar.SetActive(true);
        if (IsOwner)
        {
            // listener.enabled = true;
            vc.Priority = 10;
        }
        else
        {
            vc.Priority = 0;
        }
    }

    private void AtualizarNomesDisplay()
    {
        GameObject entidade = GameObject.FindGameObjectWithTag("Player");
        GameObject[] criancas = GameObject.FindGameObjectsWithTag("Sobrevivente");
        Debug.Log(criancas.Length);
        List<PlayerUIManager> criancasUIManagers = new();

        foreach (GameObject crianca in criancas)
        {
            criancasUIManagers.Add(crianca.GetComponent<PlayerUIManager>());
        }

        foreach (ulong playerId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerId == 0)
            {
                PlayerUIManager entUIManager = entidade.GetComponent<PlayerUIManager>();
                nomesJogadores[0].text = entUIManager.nomeJogador.Value.ToString();
            }
            else
            {
                foreach (PlayerUIManager criancaUIManager in criancasUIManagers)
                {
                    if (playerId == criancaUIManager.OwnerClientId)
                        nomesJogadores[playerId].text = criancaUIManager.nomeJogador.Value.ToString();
                }
            }
        }
    }

    private IEnumerator CheckmarksHeartBeat()
    {
        while (true)
        {
            AtualizarCheckmarksDisplay();
            yield return new WaitForSeconds(7);
        }
    }

    private void AtualizarCheckmarksDisplay()
    {
        GameObject entidade = GameObject.FindGameObjectWithTag("Player");
        GameObject[] criancas = GameObject.FindGameObjectsWithTag("Sobrevivente");
        List<PlayerUIManager> criancasUIManagers = new();

        foreach (GameObject crianca in criancas)
        {
            criancasUIManagers.Add(crianca.GetComponent<PlayerUIManager>());
        }

        foreach (ulong playerId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerId == 0)
            {
                PlayerUIManager entUIManager = entidade.GetComponent<PlayerUIManager>();
                checkmarksJogadores[0].SetActive(entUIManager.playerPronto.Value);
            }
            else
            {
                foreach (PlayerUIManager criancaUIManager in criancasUIManagers)
                {
                    if (playerId == criancaUIManager.OwnerClientId)
                        checkmarksJogadores[playerId].SetActive(criancaUIManager.playerPronto.Value);
                }
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            nomeJogador.OnValueChanged -= OnPlayerNameChanged;
            StopCoroutine(heartbeat);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReady_ServerRpc()
    {
        if(IsOwner)
            playerPronto.Value = true;
    }

    void OnPlayerNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        displayName.text = current.ToString();
    }
}
