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
        // Certifique-se de que este objeto não seja destruído quando uma nova cena for carregada.
        DontDestroyOnLoad(gameObject);

        // Inicialmente, a cena anterior será nula, pois não há cena anterior à cena inicial.
        cenaAnterior = null;
    }

    // Função para carregar a cena anterior.
    public void CarregarCenaAnterior()
    {
        if (!string.IsNullOrEmpty(cenaAnterior))
        {
            SceneManager.LoadScene(cenaAnterior);
        }
        else
        {
            Debug.LogWarning("Não há cena anterior para carregar.");
        }
    }

    // Evento chamado quando uma nova cena é carregada.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Armazena a cena atual como a cena anterior.
        cenaAnterior = scene.name;
    }
}
