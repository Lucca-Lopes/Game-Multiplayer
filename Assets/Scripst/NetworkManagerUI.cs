using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [SerializeField] TMPro.TextMeshProUGUI displayName;
    [SerializeField] GameObject interfacePlayerName;

    private void Awake()
    {
        serverButton.onClick.AddListener(() => { 
            NetworkManager.Singleton.StartServer();
        } );
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

    public void StatHostHandler()
    {
        if (GameManager.PlayerName != string.Empty)
        {
            NetworkManager.Singleton.StartHost();
            interfacePlayerName.SetActive(false);
        }
        else
            displayName.gameObject.SetActive(true);
    }

    public void StartClientHandler()
    {
        if (GameManager.PlayerName != string.Empty)
        {
            NetworkManager.Singleton.StartClient();
            interfacePlayerName.SetActive(false);
        }
        else
            displayName.gameObject.SetActive(true);
    }
}
