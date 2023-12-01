using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TimerController : NetworkBehaviour
{
    public TextMeshProUGUI timerText;
    [SerializeField] GameObject timerObj;
    private float timeRemaining = 60.0f;
    public GameObject fimdejogo;

    [SerializeField] GameObject textoVitoria;
    [SerializeField] GameObject textoDerrota;
    //[SerializeField] TextMeshProUGUI textoPontuacao;

    public static float timer
    {
        get
        {
            var timerObj = GameObject.FindGameObjectWithTag("Timer");
            return timerObj.GetComponent<TimerController>().timeRemaining;
        }
    }

    public override void OnNetworkSpawn()
    {
        GameManager.Instance.timerAtivo.OnValueChanged += StartTimer;
        if (IsOwner)
        {
            if (PlayerData.numJogadoresLobby == 2)
                timeRemaining *= PlayerData.numJogadoresLobby;
            else
                timeRemaining *= PlayerData.numJogadoresLobby - 1;
        }
    }

    private void Start()
    {
        UpdateTimerDisplay_ClientRpc(0, 0);
        //StartCoroutine(StartTimer());
    }

    public void StartTimer(bool previous, bool current)
    {
        if (current == true)
        {
            timerObj.SetActive(true);
            if (IsServer)
                StartCoroutine(RunTimer());
        }
    }

    public IEnumerator RunTimer()
    {
        int minutes;
        int seconds;
        while (timeRemaining > 0 && GameManager.Instance.timerAtivo.Value)
        {
            yield return new WaitForSeconds(1.0f);
            timeRemaining -= 1.0f;
            minutes = Mathf.FloorToInt(timeRemaining / 60);
            seconds = Mathf.FloorToInt(timeRemaining % 60);
            UpdateTimerDisplay_ClientRpc(minutes, seconds);
           
        }
        if (timeRemaining <= 0)
            timeRemaining = 0;
        minutes = Mathf.FloorToInt(timeRemaining / 60);
        seconds = Mathf.FloorToInt(timeRemaining % 60);
        UpdateTimerDisplay_ClientRpc(minutes, seconds);
        Fimdejogo_ClientRpc();
        Time.timeScale = 0;
    }

    [ClientRpc]
    private void UpdateTimerDisplay_ClientRpc(int minutes, int seconds)
    {
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); 
    }
    [ClientRpc]
    public void Fimdejogo_ClientRpc()
    {
        fimdejogo.SetActive(true);

        if (NetworkManager.Singleton.IsHost)
        {
            if (GameManager.Instance.killerWin.Value)
            {
                textoVitoria.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                textoDerrota.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            GameManager.Instance.EncerrarJogo();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            if (GameManager.Instance.killerWin.Value)
            {
                textoDerrota.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                textoVitoria.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}

