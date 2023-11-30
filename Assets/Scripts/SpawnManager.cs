using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] sobreviventes;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject entitySpawnpoint;
    public int jogadoresEsperados = 4;
    public NetworkVariable<int> jogadoresConectados = new(0);

    //private void Awake()
    //{
    //    entitySpawnpoint = GameObject.FindGameObjectWithTag("SpawnEntity");
    //    spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    //}

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
        Debug.Log($"OnNetworkSpawn - Client {NetworkManager.Singleton.LocalClientId} conectado");
        AdicionarPlayerConectado_ServerRpc(NetworkManager.Singleton.LocalClientId);
        Invoke("UpdateTimerActive_ServerRpc", 30f);
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

    /*private Vector3 RandomSurvivorSpawn()
    {
        var radSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return radSpawn.transform.position;
    }*/
}
