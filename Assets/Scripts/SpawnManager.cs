using UnityEngine;
using Unity.Netcode;
using System;

public class SpawnManager : NetworkBehaviour
{
    [Header("Spawnpoints")]
    [SerializeField] private GameObject[] sobreviventes;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject entitySpawnpoint;

    [Header("Manage de Players Prontos")]
    private int jogadoresEsperados;
    public NetworkVariable<int> jogadoresConectados = new(0);

    private void Awake()
    {
        jogadoresEsperados = PlayerData.numJogadoresLobby;
        jogadoresConectados.OnValueChanged += JogadoresContectadosHandler;
    }

    private void JogadoresContectadosHandler(int previousValue, int newValue)
    {
        Debug.Log($"{newValue} jogadores conectados");
        Debug.Log($"jogadores esperados - {jogadoresEsperados}");
        if (jogadoresEsperados == newValue)
        {
            if (IsHost)
            {
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, true);
            }
            else if (IsClient)
            {
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, false);
            }
            if (IsServer)
            {
                InvokeRepeating(nameof(UpdateTimerActive_ServerRpc), 3f, 1f);
            }
        }
            //Invoke(nameof(UpdateTimerActive_ServerRpc), 50f);
    }

    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        /*if (IsHost)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, true);
        }
        else if (IsClient)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, false);
        }*/
        AdicionarPlayerConectado_ServerRpc();
        //Invoke(nameof(UpdateTimerActive_ServerRpc), 50f);
        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AdicionarPlayerConectado_ServerRpc()
    {
        jogadoresConectados.Value += 1;
        Debug.Log($"{jogadoresConectados.Value} jogadores conectados");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateTimerActive_ServerRpc()
    {
        if (GameManager.Instance.timerAtivo.Value == true)
        {
            CancelInvoke(nameof(UpdateTimerActive_ServerRpc));
            return;
        }
        for(int i = 0; i < jogadoresEsperados; i++)
        {
            if (NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject == null)
                return;
        }
        GameManager.Instance.timerAtivo.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId, bool isEntity)
    {
        GameObject newPlayer;
        if (isEntity)
        {
            newPlayer = Instantiate(entity, entitySpawnpoint.transform.position, entitySpawnpoint.transform.rotation);
        }
        else 
        {
            newPlayer = Instantiate(sobreviventes[clientId-1]);
        }
        var netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
        netObj.ChangeOwnership(clientId);
    }
}
