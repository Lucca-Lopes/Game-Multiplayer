using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

public class Reinicar : MonoBehaviour
{
    [SerializeField] private Button stopHostButton;
    public void VoltarMenuInicial()
    {
        Time.timeScale = 1;
        // Obt�m o nome da cena atual e recarrega a cena
        //string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(0);
    }

    private void Awake()
    {
        stopHostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
        });
    }

}
