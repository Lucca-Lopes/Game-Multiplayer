using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Reinicar : MonoBehaviour
{
    public void RestartScene()
    {
        // Obtém o nome da cena atual e recarrega a cena
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1;
    }
}
