using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{

    [SerializeField] private GameObject sobrevivente;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject entitySpawnpoint;

    //private void Awake()
    //{
    //    entitySpawnpoint = GameObject.FindGameObjectWithTag("SpawnEntity");
    //    spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    //}

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        }
        else if (IsClient)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
        }
        base.OnNetworkSpawn();
    }

    //public void SpawnPlayers()
    //{
    //    foreach (ulong playerId in NetworkManager.Singleton.ConnectedClientsIds)
    //    {
    //        if (playerId.Equals(0))
    //        {
    //            SpawnPlayerServerRpc(playerId, 0);
    //        }
    //        else
    //        {
    //            SpawnPlayerServerRpc(playerId, 1);
    //        }
    //    }
    //}

    //void InstanciarAssassino()
    //{
    //    if (entity == null)
    //    {
    //        entity = Resources.Load<GameObject>("Prefabs/Entity");
    //    }
    //    var entidadeInstanciada = Instantiate(entity, transform);
    //    GameManager.AddPlayer(OwnerClientId);
    //    transform.position = entitySpawnpoint.transform.position;
    //    entidadeInstanciada.GetComponent<NetworkObject>().Spawn();
    //}

    //void InstanciarSobrevivente()
    //{
    //    if (sobrevivente == null)
    //    {
    //        sobrevivente = Resources.Load<GameObject>("Prefabs/Player");
    //    }
    //    var sobreviventeInstanciado = Instantiate(sobrevivente, transform);
    //    GameManager.AddPlayer(OwnerClientId, sobreviventeInstanciado.GetComponent<Personagem>());
    //    transform.position = RandomSurvivorSpawn();
    //    GameManager.Instance.SpawnSobrevivente_ServerRpc(OwnerClientId);
    //}



    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId, int prefabId)
    {
        GameObject newPlayer;
        if (prefabId == 0)
        {
            newPlayer = Instantiate(entity);
            newPlayer.transform.position = entitySpawnpoint.transform.position;
        }
        else
        {
            newPlayer = Instantiate(sobrevivente);
            newPlayer.transform.position = RandomSurvivorSpawn();
        }
        var netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
        netObj.ChangeOwnership(clientId);
        GameManager.FetchPlayers();
    }

    private Vector3 RandomSurvivorSpawn()
    {
        var radSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return radSpawn.transform.position;
    }
}
