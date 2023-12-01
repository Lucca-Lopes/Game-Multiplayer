using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

public class Reinicar : MonoBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void DisconnectPlayers_ServerRpc()
    {
        NetworkManager.Singleton.StopAllCoroutines();
        //Destroy(GameObject.FindGameObjectWithTag("NetworkManager"));
        //foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        //{
        //    NetworkManager.Singleton.DisconnectClient(clientId);
        //}
        NetworkManager.Singleton.Shutdown();
    }
}
