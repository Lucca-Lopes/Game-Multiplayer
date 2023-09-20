using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject entitySpawnpoint;
    private int radNumInt;

    void Start()
    {
        if(spawnPoints.Length == 0)
        {
            spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        }

        if(entitySpawnpoint == null)
        {
            entitySpawnpoint = GameObject.FindGameObjectWithTag("SpawnEntity");
        }

        if (player == null)
        {
            player = Resources.Load<GameObject>("Prefabs/Player");
        }

        if (entity == null)
        {
            entity = Resources.Load<GameObject>("Prefabs/Entity");
        }

        radNumInt = RandomNumber();

        Instantiate(entity, entitySpawnpoint.transform.position, entitySpawnpoint.transform.rotation);


        Instantiate(player, spawnPoints[radNumInt].transform.position, spawnPoints[0].transform.rotation);
    }

    private int RandomNumber()
    {
        float radNumFloat = 0f;
        radNumFloat = Random.Range(0, 6);
        Debug.Log(radNumFloat);
        return (int)radNumFloat;
    }
}
