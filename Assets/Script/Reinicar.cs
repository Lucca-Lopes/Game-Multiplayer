using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

public class Reinicar : MonoBehaviour
{
    [SerializeField] private Button stopHostButton;
    public void RestartScene()
    {
        // Obtém o nome da cena atual e recarrega a cena
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        
        Time.timeScale = 1;
    }

    private void Awake()
    {
        stopHostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
        });
    }

}
