using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    //[SerializeField] private Button serverButton;
    //[SerializeField] private Button hostButton;
    //[SerializeField] private Button clientButton;

    [SerializeField] GameObject errorText;
    [SerializeField] TMPro.TextMeshProUGUI playerName;
    [SerializeField] GameObject interfacePlayerName;

    //private void Awake()
    //{
    //    serverButton.onClick.AddListener(() => { 
    //        NetworkManager.Singleton.StartServer();
    //    } );
    //    hostButton.onClick.AddListener(() => {
    //        NetworkManager.Singleton.StartHost();
    //    });
    //    clientButton.onClick.AddListener(() => {
    //        NetworkManager.Singleton.StartClient();
    //    });
    //}

    public void StartHostHandler()
    {
        if (playerName.text != string.Empty)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Nome player: " + playerName.text);
            interfacePlayerName.SetActive(false);
        }
        else
            errorText.SetActive(true);
    }

    public void StartClientHandler()
    {
        if (playerName.text != string.Empty)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Nome player: " + playerName.text);
            interfacePlayerName.SetActive(false);
        }
        else
            errorText.SetActive(true);
    }
}
