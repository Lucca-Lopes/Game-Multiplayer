using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class WebRequest : MonoBehaviour
{
    public Player[] players;
    //Coroutine getCoroutine, postCoroutine, deleteCoroutine;
    public UnityEvent<RankingData> OnListUpdate;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest("http://localhost:3000/ranking"));
    }

    IEnumerator GetRequest(string url)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result.Equals(UnityWebRequest.Result.Success))
        {
            RankingData ranking = JsonUtility.FromJson<RankingData>(request.downloadHandler.text);
            OnListUpdate?.Invoke(ranking);
        }
    }

    IEnumerator PostData(string url, Player newPlayer)
    {
        WWWForm playerForm = new();
        playerForm.AddField("id", newPlayer.id);
        playerForm.AddField("nome", newPlayer.nome);
        playerForm.AddField("pontuacao", newPlayer.pontuacao);

        using UnityWebRequest request = UnityWebRequest.Post(url, playerForm);
        yield return request.SendWebRequest();
        StartCoroutine(GetRequest(url));
    }

    IEnumerator DeleteData(string url, int idPlayerDeletado)
    {
        var urlDelete = url + "/" + idPlayerDeletado;
        using UnityWebRequest request = UnityWebRequest.Delete(urlDelete);

        yield return request.SendWebRequest();
        StartCoroutine(GetRequest(url));
    }

}
