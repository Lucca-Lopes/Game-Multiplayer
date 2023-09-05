using System.IO;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Esta classe implementa todas as chamadas de servi�o com o servidor 
/// <remarks>
/// <para>
/// Servi�os
/// <list type="bullet">
/// <item>GetRanking</item>
/// <item>PostNewPlayer</item>
/// <item>DeletePlayer</item>
/// </list>
/// 
/// Esta classe conta com um evento que � disparado toda vez que a 
/// lista de jogadores � atualizada.
/// 
/// <see cref="OnListUpdate"/>
/// 
/// Esta classe utiliza a classe <seealso cref="RestUtil"/>
/// 
/// </para>
/// </remarks>
/// </summary>
public class RankingService : MonoBehaviour
{
    [Header("Service configuration")]
    [Tooltip("Dominio do servi�o (URI)")]
    [SerializeField] string domain;
    [SerializeField] string getService;
    [SerializeField] string setService;
    [SerializeField] string postService;
    [SerializeField] string deleteService;
    [Tooltip("Informa se � para atualizar de forma autom�tica a lista de jogadores")]
    [SerializeField] bool autoUpdate = false;
    
    [Space]
    [Header("Events")]
    [Tooltip("Este evento ser� chamado sempre que a lista sofrer altera��es")]
    public UnityEvent<RankingData> OnListUpdate;

    /// <summary>
    /// <remarks>
    /// <para>
    /// Estas vari�veis armazenar�o os IDs das coroutinas chamadas para cada servir�o
    /// </para>
    /// <para>
    /// Os servi�os s�o ass�ncronos, desse modo, cada vez que chamamos um servi�o ele libera o processo
    /// e sua resposta chega atrav�s de eventos em outro momento.
    /// </para>
    /// <para>
    /// Sabendo disso essas vari�veis armazenam os IDs de cada corotina e as destroi sempre que um retorno
    /// de evento � recebido em seu handler.
    /// </para>
    /// </remarks>
    /// </summary>
    Coroutine getCoroutine, postCoroutine, deleteCoroutine;

    #region Actions
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo recuperar o ranking atualizado no servidor
    /// <see cref="RestUtil.RequestData(string, UnityAction{string})"/>
    /// </para>
    /// </remarks>
    /// </summary>
    public void GetRanking()
    {
        string url = string.Concat(domain, RestUtil.UrlSeparator, getService);
        getCoroutine = StartCoroutine(RestUtil.RequestData(url, GetRankingHandler));
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo salva as informa��es de um novo jogador no ranking
    /// </para>
    /// <para>
    /// Este m�todo recebe uma classe <see cref="Player"/> que representa as informa��es do jogador
    /// </para>
    /// <para>
    /// As informa��es do player � gerada pelo formul�rio existente na tela
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="newPlayer">Dados do Jogador</param>
    public void PostNewPlayer(Player newPlayer)
    {
        string url = string.Concat(domain, RestUtil.UrlSeparator, getService);
        postCoroutine = StartCoroutine(
            RestUtil.PostData(url, newPlayer, PostRankingHandler)
        );
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo apaga as informa��es de um jogador no ranking
    /// </para>
    /// <para>
    /// Esta informa��o vem pela UI do ranking quando clica no �cone de apagar
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="player"></param>
    public void DeletePlayer(int player)
    {
        if(player > 0)
        {
            string url = string.Concat(domain, RestUtil.UrlSeparator, deleteService, RestUtil.UrlSeparator, player);
            Debug.Log("Delete url: " + url);
            deleteCoroutine = StartCoroutine(RestUtil.DeleteData(url, DeleteRankingHandler));
        }
    }
    #endregion Actions

    #region Handlers
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo � o hendler que trata o retorno do servi�o de recuperar o ranking <see cref="GetRanking"/>
    /// </para>
    /// <para>
    /// A string JSON recebida � convertida na classe <see cref="RankingData"/>
    /// s� depois � disparado o evento <seealso cref="OnListUpdate"/>
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="jsonData">Este � o ranking atualizado no formato JSON</param>
    void GetRankingHandler(string jsonData)
    {
        //Para a coroutine criada e armazenada na vari�vel getCoroutine
        StopCoroutine(getCoroutine);
        //Converte a string JSON recebida do servi�o
        RankingData ranking = JsonUtility.FromJson<RankingData>(jsonData);
        //Dispara o evento OnListUpdate, ele s� � disparado caso n�o seja NULL. O ponto de interroga��o � exatamente para isso
        OnListUpdate?.Invoke(ranking);
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo � o hendler que trata o retorno do servi�o de Novo player <see cref="PostNewPlayer(Player)"/>
    /// </para>
    /// <para>
    /// Se a op��o <see cref="autoUpdate"/> estiver marcada no inspetor este servi�o chamar� automaticamente o m�todo <see cref="GetRanking"/>
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="result"></param>
    void PostRankingHandler(int result)
    {
        //Para a coroutine criada e armazenada na vari�vel postCoroutine
        StopCoroutine(postCoroutine);
        //Chama o evento de GetRanking para atualizar a lista do ranking na tela
        if (autoUpdate) GetRanking();
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo � o hendler que trata o retorno do servi�o de pagar um player do ranking <see cref="DeletePlayer(int)"/>
    /// </para>
    /// <para>
    /// Se a op��o <see cref="autoUpdate"/> estiver marcada no inspetor este servi�o chamar� automaticamente o m�todo <see cref="GetRanking"/>
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="result"></param>
    void DeleteRankingHandler(int result)
    {
        //Para a coroutine criada e armazenada na vari�vel deleteCoroutine
        StopCoroutine(deleteCoroutine);
        //Chama o evento de GetRanking para atualizar a lista do ranking na tela
        if (autoUpdate) GetRanking();
    }
    #endregion Handlers
}
