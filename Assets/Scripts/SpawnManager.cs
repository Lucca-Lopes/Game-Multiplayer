using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{

    [SerializeField] private GameObject sobrevivente;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject entitySpawnpoint;

    private void Awake()
    {
        entitySpawnpoint = GameObject.FindGameObjectWithTag("SpawnEntity");
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsHost)
        {
            InstanciarAssassino();
        }
        else if (IsOwner && IsClient)
        {
            InstanciarSobrevivente();
        }
        
        base.OnNetworkSpawn();
    }

    void InstanciarAssassino()
    {
        if (entity == null)
        {
            entity = Resources.Load<GameObject>("Prefabs/Entity");
        }
        Instantiate(entity, gameObject.transform);
        //GameManager.AddPlayer(OwnerClientId);
        transform.position = entitySpawnpoint.transform.position;
    }

    void InstanciarSobrevivente()
    {
        if (sobrevivente == null)
        {
            sobrevivente = Resources.Load<GameObject>("Prefabs/Player");
        }
        var sobreviventeInstanciado = Instantiate(sobrevivente, transform);
        //GameManager.AddPlayer(OwnerClientId, sobreviventeInstanciado.GetComponent<Personagem>());
        transform.position = RandomSurvivorSpawn();
    }

    private Vector3 RandomSurvivorSpawn()
    {
        var radSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return radSpawn.transform.position;
    }
}
