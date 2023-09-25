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
    [SerializeField] TMPro.TMP_InputField playerName;
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
        if (!IsInputFieldEmpty(playerName))
        {
            GameManager.PlayerName = playerName.text;
            NetworkManager.Singleton.StartHost();
            interfacePlayerName.SetActive(false);
        }
        else
            errorText.SetActive(true);
    }

    public void StartClientHandler()
    {
        if (!IsInputFieldEmpty(playerName))
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) {
                GameManager.PlayerName = playerName.text;
                NetworkManager.Singleton.StartClient();
                interfacePlayerName.SetActive(false);
            }
        }
        else
            errorText.SetActive(true);
    }

    bool IsStringEmpty(string value)
    {
        return string.IsNullOrEmpty(value);
    }

    bool IsInputFieldEmpty(TMPro.TMP_InputField inputField)
    {
        return IsStringEmpty(inputField.text);
    }
}
