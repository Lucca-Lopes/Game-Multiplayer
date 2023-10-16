using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class FakeLobby : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyText;
    //public NetworkVariable<int> numeroDeJogadores = new(0);

    private void Update()
    {
        if (IsClient)
        {
            if (GameManager.Instance.jogadoresConectados.Count < 4)
            {
                lobbyText.gameObject.SetActive(true);
                lobbyText.text = "Esperando jogadores... (" + GameManager.Instance.jogadoresConectados.Count + "/4)";
                Time.timeScale = 0;
            }
            else
            {
                lobbyText.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}
