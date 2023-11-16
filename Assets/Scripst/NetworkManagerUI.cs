using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("Configurações Input de Nome")]
    [SerializeField] GameObject errorTextInput;
    [SerializeField] TMP_InputField playerName;
    [SerializeField] GameObject interfacePlayerName;
    [SerializeField] GameObject interfaceLore;

    public void StartHostHandler()
    {
        if (!IsInputFieldEmpty(playerName))
        {
            GameManager.PlayerName = playerName.text;
            NetworkManager.Singleton.StartHost();
            interfaceLore.SetActive(true);
            interfacePlayerName.SetActive(false);
        }
        else
            errorTextInput.SetActive(true);
    }

    public void StartClientHandler()
    {
        if (!IsInputFieldEmpty(playerName))
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) {
                GameManager.PlayerName = playerName.text;
                NetworkManager.Singleton.StartClient();
                interfaceLore.SetActive(true);
                interfacePlayerName.SetActive(false);
            }
        }
        else
            errorTextInput.SetActive(true);
    }

    bool IsStringEmpty(string value)
    {
        return string.IsNullOrEmpty(value);
    }

    bool IsInputFieldEmpty(TMP_InputField inputField)
    {
        return IsStringEmpty(inputField.text);
    }
}
