using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining = 2 * 60.0f;
    public GameObject fimdejogo;

    private void Start()
    {
        UpdateTimerDisplay();
        //StartCoroutine(StartTimer());
    }

    public IEnumerator StartTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeRemaining -= 1.0f;
            UpdateTimerDisplay();
           
        }
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
        
    }
}

