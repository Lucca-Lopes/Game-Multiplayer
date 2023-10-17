using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

public class Reinicar : NetworkBehaviour
{
    [SerializeField] GameObject telaInicial;
    [SerializeField] GameObject telaFimDeJogo;

    public void VoltarMenuInicial()
    {
        telaInicial.SetActive(true);
        telaFimDeJogo.SetActive(false);
        Time.timeScale = 1;
        NetworkManager.Singleton.StopAllCoroutines();
        if (IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
