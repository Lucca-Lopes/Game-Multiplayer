using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TimerController : NetworkBehaviour
{
    public TextMeshProUGUI timerText;
    [SerializeField] GameObject timerObj;
    private float timeRemaining = 2 * 60.0f;
    public GameObject fimdejogo;

    [SerializeField] GameObject textoVitoria;
    [SerializeField] GameObject textoDerrota;
    [SerializeField] TextMeshProUGUI textoPontuacao;

    public override void OnNetworkSpawn()
    {
        GameManager.Instance.timerAtivo.OnValueChanged += StartTimer;
    }

    private void Start()
    {
        UpdateTimerDisplay();
        //StartCoroutine(StartTimer());
    }

    public void StartTimer(bool previous, bool current)
    {
        if (current == true)
        {
            timerObj.SetActive(true);
            StartCoroutine(RunTimer());
        }
    }

    public IEnumerator RunTimer()
    {
        while (timeRemaining > 0 && GameManager.Instance.timerAtivo.Value)
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
        fimdejogo.SetActive(true);
        if (NetworkManager.Singleton.IsHost)
        {
            if (GameManager.Instance.killerWin.Value)
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
            if (GameManager.Instance.killerWin.Value)
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

