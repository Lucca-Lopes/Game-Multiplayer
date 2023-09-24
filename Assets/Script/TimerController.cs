using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining = 2 * 60.0f;
    public GameObject fimdejogo;
    [SerializeField] GameObject textoVitoria;
    [SerializeField] GameObject textoDerrota;
    [SerializeField] TextMeshProUGUI textoPontuacao;

    private void Start()
    {
        UpdateTimerDisplay();
        //StartCoroutine(StartTimer());
    }

    public IEnumerator StartTimer()
    {
        while (timeRemaining > 0 && !GameManager.fimDeJogo)
        {
            yield return new WaitForSeconds(1.0f);
            timeRemaining -= 1.0f;
            UpdateTimerDisplay();
           
        }
        if (timeRemaining <= 0)
            timeRemaining = 0;
        UpdateTimerDisplay();
        Fimdejogo();
        Time.timeScale = 0;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); 
    }
    public void Fimdejogo()
    {
        GameManager.fimDeJogo = true;
        fimdejogo.SetActive(true);
        if (NetworkManager.Singleton.IsHost)
        {
            if (GameManager.killerWin)
            {
                textoVitoria.SetActive(true);
            }
            else
            {
                textoDerrota.SetActive(true);
            }
            textoPontuacao.text = "Sua pontuação: " + timeRemaining;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            if (GameManager.killerWin)
            {
                textoDerrota.SetActive(true);
            }
            else
            {
                textoVitoria.SetActive(true);
            }
            textoPontuacao.text = "Sua pontuação: " + (120 - timeRemaining);
        }
    }
}

