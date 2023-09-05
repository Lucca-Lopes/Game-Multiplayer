using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
/// <summary>
/// <remarks>
/// <para>
/// Esta classe cria uma abstra��o para a execu��o dos servi�os
/// </para>
/// <para>
/// Ela disponibiliza m�todos est�ticos que precisam receber as informa��es 
/// b�sicas para execu��o do servi�o e encapsula todo o restante do trabalho
/// </para>
/// <para>
/// Os servi�os s�o:
/// <list type="table">
///     <listheader>
///         <term>Nome do servi�o</term>
///         <description>Descri��o do servi�o</description>
///     </listheader>
///     <item>
///         <term><see cref="RequestData(string, UnityEngine.Events.UnityAction{string})"/></term>
///         <description>Executa uma chamada de servi�o via m�todo Get</description>
///     </item>
///     <item>
///         <term><see cref="PostData(string, Player, UnityEngine.Events.UnityAction{int})"/></term>
///         <description>Executa uma chamada de servi�o via m�todo Post</description>
///     </item>
///     <item>
///         <term><see cref="DeleteData(string, UnityEngine.Events.UnityAction{int})"/></term>
///         <description>Executa uma chamada de servi�o via m�todo Delete</description>
///     </item>
/// </list>
/// </para>
/// 
/// </remarks>
/// </summary>
public class RestUtil : MonoBehaviour
{
    /// <summary>
    /// Caracter utilizado para separar a url
    /// </summary>
    public static string UrlSeparator = "/";
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo executa uma chamada para para a url informada no parametro url e executa um callback, caso seja informado um no par�metro callback
    /// </para>
    /// <para>
    /// A chamada da URL � realizada atrav�s da classe <see cref="UnityWebRequest.Get(string)"/>
    /// </para>
    /// <para>
    /// OBS.: Este m�todo possui um tipo de retorno <see cref="IEnumerator"/> o que permite realizar uma chamada de Coroutine e n�o prendendo o processo
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="url">URL completa que deve ser chamada pelo m�todo <see cref="UnityWebRequest.Get(string)"/></param>
    /// <param name="callback">Callback que ser� chamado como retorno da URL, caso retorne algo <see cref="UnityAction{T0}"/></param>
    /// <returns>IEnumerator</returns>
    public static IEnumerator RequestData(string url, UnityAction<string> callback = null)
    {
        Debug.Log("url " + url);
        /*
         * O uso do comando using garante que ser� criada uma
        * vari�vel preparada para a utiliza��o correta das
        * interfaces IAsyncDisposable (a partir do C# 8.0) ou IDisposable em vers�es anteriores
        */
        using UnityWebRequest request = UnityWebRequest.Get(url);
        Debug.Log("Chamando os servi�o de GET");
        // Chama o servi�o e espera o retorno do m�todo SendWebRequest
        yield return request.SendWebRequest();
        Debug.Log("Retorno do servi�o de GET");
        Debug.Log("Status " + request.result);
        Debug.Log("Status " + request.downloadHandler.text);
        //Verifica se a requisi��o foi executada com sucesso
        if (request.result.Equals(UnityWebRequest.Result.Success))
        {
            //Chama o callback informado, caso n�o seja nulo. 
            //Envia o retorno recebido do servi�o, este vem em um formato de stringo JSON
            callback?.Invoke(request.downloadHandler.text);
        }
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo executa uma chamada para para a url informada no parametro url e executa um callback, caso seja informado um no par�metro callback
    /// </para>
    /// <para>
    /// A chamada da URL � realizada atrav�s da classe <see cref="UnityWebRequest.Post(string, WWWForm)"/>
    /// </para>
    /// <para>
    /// OBS.: Este m�todo possui um tipo de retorno <see cref="IEnumerator"/> o que permite realizar uma chamada de Coroutine e n�o prendendo o processo
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="url">URL completa que deve ser chamada pelo m�todo <see cref="UnityWebRequest.Post(string, WWWForm)"/></param>
    /// <param name="newPlayer">Dados do jogador <see cref="Player"/></param>
    /// <param name="callback">Callback que ser� chamado como retorno da URL, caso retorne algo <see cref="UnityAction{T0}"/></param>
    /// <returns></returns>
    public static IEnumerator PostData(string url, Player newPlayer, UnityAction<int> callback = null)
    {
        //Cria um objeto WWWForm para carregar os dados do jogador atrav�s da chamada do servi�o
        //Esta classe funciona como um Dictionary e nos permite criar conjuntos de chave e valor
        WWWForm playerForm = new WWWForm();
        //Armazenado os dados do jogador.
        playerForm.AddField("id", newPlayer.id);
        playerForm.AddField("nome", newPlayer.nome);
        playerForm.AddField("pontuacao", newPlayer.pontuacao);
        /*
         * O uso do comando using garante que ser� criada uma
        * vari�vel preparada para a utiliza��o correta das
        * interfaces IAsyncDisposable (a partir do C# 8.0) ou IDisposable em vers�es anteriores
        */
        using UnityWebRequest request = UnityWebRequest.Post(url, playerForm);
        // Chama o servi�o e espera o retorno do m�todo SendWebRequest
        yield return request.SendWebRequest();
        //Verifica se a requisi��o foi executada com sucesso
        if (request.result.Equals(UnityWebRequest.Result.Success))
        {
            //Chama o callback informado, caso n�o seja nulo
            callback?.Invoke(200);
        }
    }
    /// <summary>
    /// <remarks>
    /// <para>
    /// Este m�todo executa uma chamada para para a url informada no parametro url e executa um callback, caso seja informado um no par�metro callback
    /// </para>
    /// <para>
    /// A chamada da URL � realizada atrav�s da classe <see cref="UnityWebRequest.Delete(string)"/>
    /// </para>
    /// <para>
    /// OBS.: Este m�todo possui um tipo de retorno <see cref="IEnumerator"/> o que permite realizar uma chamada de Coroutine e n�o prendendo o processo
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="url">URL completa que deve ser chamada pelo m�todo <see cref="UnityWebRequest.Get(string)"/></param>
    /// <param name="callback">Callback que ser� chamado como retorno da URL, caso retorne algo <see cref="UnityAction{T0}"/></param>
    /// <returns>IEnumerator</returns>
    public static IEnumerator DeleteData(string url, UnityAction<int> callback = null)
    {
        using UnityWebRequest request = UnityWebRequest.Delete(url);

        yield return request.SendWebRequest();

        if (request.result.Equals(UnityWebRequest.Result.Success))
        {
            callback?.Invoke(200);
        }
    }
}
