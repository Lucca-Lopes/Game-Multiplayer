using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoltarCena : MonoBehaviour
{
    //Este codigo Ainda esta em testes


    private string cenaAnterior;

    private void Awake()
    {
        // Certifique-se de que este objeto n�o seja destru�do quando uma nova cena for carregada.
        DontDestroyOnLoad(gameObject);

        // Inicialmente, a cena anterior ser� nula, pois n�o h� cena anterior � cena inicial.
        cenaAnterior = null;
    }

    // Fun��o para carregar a cena anterior.
    public void CarregarCenaAnterior()
    {
        if (!string.IsNullOrEmpty(cenaAnterior))
        {
            SceneManager.LoadScene(cenaAnterior);
        }
        else
        {
            Debug.LogWarning("N�o h� cena anterior para carregar.");
        }
    }

    // Evento chamado quando uma nova cena � carregada.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Armazena a cena atual como a cena anterior.
        cenaAnterior = scene.name;
    }
}
