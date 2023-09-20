using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining = 2 * 60.0f; // 2 minutos em segundos

    private void Start()
    {
        UpdateTimerDisplay();
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeRemaining -= 1.0f;
            UpdateTimerDisplay();
        }
        timeRemaining = 0; // Certifica-se de que o timer seja 0 quando terminar
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Atualiza o TextMeshPro com o tempo restante formatado como MM:SS
    }
}

