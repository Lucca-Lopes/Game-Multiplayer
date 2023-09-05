using System.IO;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Esta classe implementa todas as chamadas de serviço com o servidor 
/// <remarks>
/// <para>
/// Serviços
/// <list type="bullet">
/// <item>GetRanking</item>
/// <item>PostNewPlayer</item>
/// <item>DeletePlayer</item>
/// </list>
/// 
/// Esta classe conta com um evento que é disparado toda vez que a 
/// lista de jogadores é atualizada.
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
    [Tooltip("Dominio do serviço (URI)")]
    [SerializeField] string domain;
    [SerializeField] string getService;
    [SerializeField] string setService;
    [SerializeField] string postService;
    [SerializeField] string deleteService;
    [Tooltip("Informa se é para atualizar de forma automática a lista de jogadores")]
    [SerializeField] bool autoUpdate = false;
    
    [Space]
    [Header("Events")]
    [Tooltip("Este evento será chamado sempre que a lista sofrer alterações")]
    public UnityEvent<RankingData> OnListUpdate;

    /// <summary>
    /// <remarks>
    /// <para>
    /// Estas variáveis armazenarão os IDs das coroutinas chamadas para cada servirço
    /// </para>
    /// <para>
    /// Os serviços são assíncronos, desse modo, cada vez que chamamos um serviço ele libera o processo
    /// e sua resposta chega através de eventos em outro momento.
    /// </para>
    /// <para>
    /// Sabendo disso essas variáveis armazenam os IDs de cada corotina e as destroi sempre que um retorno
    /// de evento é recebido em seu handler.
    /// </para>
    /// </remarks>
    /// </summary>
    Coroutine getCoroutine, postCoroutine, deleteCoroutine;

    #region Actions
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este método recuperar o ranking atualizado no servidor
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
    /// Este método salva as informações de um novo jogador no ranking
    /// </para>
    /// <para>
    /// Este método recebe uma classe <see cref="Player"/> que representa as informações do jogador
    /// </para>
    /// <para>
    /// As informações do player é gerada pelo formulário existente na tela
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
    /// Este método apaga as informações de um jogador no ranking
    /// </para>
    /// <para>
    /// Esta informação vem pela UI do ranking quando clica no ícone de apagar
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
    /// Este método é o hendler que trata o retorno do serviço de recuperar o ranking <see cref="GetRanking"/>
    /// </para>
    /// <para>
    /// A string JSON recebida é convertida na classe <see cref="RankingData"/>
    /// só depois é disparado o evento <seealso cref="OnListUpdate"/>
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="jsonData">Este é o ranking atualizado no formato JSON</param>
    void GetRankingHandler(string jsonData)
    {
        //Para a coroutine criada e armazenada na variável getCoroutine
        StopCoroutine(getCoroutine);
        //Converte a string JSON recebida do serviço
        RankingData ranking = JsonUtility.FromJson<RankingData>(jsonData);
        //Dispara o evento OnListUpdate, ele só é disparado caso não seja NULL. O ponto de interrogação é exatamente para isso
        OnListUpdate?.Invoke(ranking);
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este método é o hendler que trata o retorno do serviço de Novo player <see cref="PostNewPlayer(Player)"/>
    /// </para>
    /// <para>
    /// Se a opção <see cref="autoUpdate"/> estiver marcada no inspetor este serviço chamará automaticamente o método <see cref="GetRanking"/>
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="result"></param>
    void PostRankingHandler(int result)
    {
        //Para a coroutine criada e armazenada na variável postCoroutine
        StopCoroutine(postCoroutine);
        //Chama o evento de GetRanking para atualizar a lista do ranking na tela
        if (autoUpdate) GetRanking();
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este método é o hendler que trata o retorno do serviço de pagar um player do ranking <see cref="DeletePlayer(int)"/>
    /// </para>
    /// <para>
    /// Se a opção <see cref="autoUpdate"/> estiver marcada no inspetor este serviço chamará automaticamente o método <see cref="GetRanking"/>
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="result"></param>
    void DeleteRankingHandler(int result)
    {
        //Para a coroutine criada e armazenada na variável deleteCoroutine
        StopCoroutine(deleteCoroutine);
        //Chama o evento de GetRanking para atualizar a lista do ranking na tela
        if (autoUpdate) GetRanking();
    }
    #endregion Handlers
}
