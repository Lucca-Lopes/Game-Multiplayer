using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("Configurações Input de Nome")]
    [SerializeField] GameObject errorTextInput;
    [SerializeField] TMP_InputField playerName;
    [SerializeField] GameObject interfacePlayerName;

    public void StartHostHandler()
    {
        if (!IsInputFieldEmpty(playerName))
        {
            GameManager.PlayerName = playerName.text;
            NetworkManager.Singleton.StartHost();
            interfacePlayerName.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
                interfacePlayerName.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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
