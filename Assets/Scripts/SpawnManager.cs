using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
    [Header("Spawnpoints")]
    [SerializeField] private GameObject[] sobreviventes;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject entitySpawnpoint;

    [Header("Manage de Players Prontos")]
    public int jogadoresEsperados = 4;
    public NetworkVariable<int> jogadoresConectados = new(0);

    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (IsHost)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, true);
        }
        else if (IsClient)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, false);
        }
        AdicionarPlayerConectado_ServerRpc(NetworkManager.Singleton.LocalClientId);
        Invoke(nameof(UpdateTimerActive_ServerRpc), 20f);
        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AdicionarPlayerConectado_ServerRpc(ulong clientId)
    {
        jogadoresConectados.Value += 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateTimerActive_ServerRpc()
    {
        if (jogadoresEsperados <= jogadoresConectados.Value)
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
