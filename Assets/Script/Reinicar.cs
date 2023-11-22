using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

public class Reinicar : NetworkBehaviour
{
    public void VoltarMenuInicial()
    {
        Time.timeScale = 1;
        NetworkManager.Singleton.StopAllCoroutines();
        if (IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.SceneManager.LoadScene("MenuLobby", LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.SceneManager.LoadScene("MenuLobby", LoadSceneMode.Single);
        }

    }
}
