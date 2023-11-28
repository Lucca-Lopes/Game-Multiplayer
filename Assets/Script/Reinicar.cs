using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

public class Reinicar : MonoBehaviour
{
    public void VoltarMenuInicial()
    {
        Time.timeScale = 1;
        NetworkManager.Singleton.StopAllCoroutines();
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        }
        Destroy(gameObject);
        SceneManager.LoadScene("MenuLobby", LoadSceneMode.Single);
    }
}
