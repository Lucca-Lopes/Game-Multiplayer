using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
    [Header("Service")]
    public RankingService service;
    
    [Space]
    [Header("Ranking List")]
    public GameObject rankingContainer;
    public RankingEntryUI rankingEntry;

    [Space]
    [Header("Save Player")]
    public TMPro.TMP_InputField inputId;
    public TMPro.TMP_InputField inputNome;
    public TMPro.TMP_InputField inputPonto;

    //Armazena internamente o id de todos os players que estão no ranking
    List<int> playersId;

    private void Start()
    {
        //Iniciando a lista de ids
        playersId = new List<int>();
        //Preparando o Handler do evento para atualizar o ranking na tela
        service.OnListUpdate.AddListener(UpdateRanking);
        //Chamando o serviço de recuperar o ranking assim que começa a cena
        service.GetRanking();
    }
    /// <summary>
    /// <remarks>
    /// <para>Este método é chamado sempre que a lista do ranking é atualizada</para>
    /// Isso significa que ao ser executado um serviço que atualiza a lista de ranking e a opção <see cref="RankingService.autoUpdate"/> estiver marcada no editor 
    /// este método será chamado e receberá a nova lista de players no ranking.
    /// <para>Com essa lista o método atualiza a interface e recria os prefabs de player na tela</para>
    /// Para isso é utilizado o método <see cref="ClearContainer"/> e <see cref="DisplayList(RankingData)"/>
    /// </remarks>
    /// Este método é chamado sempre que o ranking 
    /// </summary>
    /// <param name="data">Dados do Ranking <see cref="RankingData"/></param>
    public void UpdateRanking(RankingData data)
    {
        Debug.Log("Data: " + data.players);
        if (data != null)
        {
            //Limpando a lista
            ClearContainer();

            //Recriando a lista
            DisplayList(data);
        }
    }

    #region Private Methods
    /// <summary>
    /// Este método recria, com base no ranking informado, a lista de jogadores no ranking
    /// </summary>
    /// <param name="data"><see cref="RankingData"/></param>
    private void DisplayList(RankingData data)
    {
        RankingEntryUI temp;

        //Faz um loop na lista de jogadores, porém, antes ele reordena a lista para que seja do maior para o menor utilizando a biblioteca Linq do C#
        foreach (Player entry in data.players.OrderByDescending(player => player.pontuacao))
        {
            //Cria a instância do prefab do player na tela
            temp = Instantiate(rankingEntry, rankingContainer.transform);
            //Atribui os dados do jogador 
            temp.ID = entry.id;
            temp.Nome = entry.nome;
            temp.Pontos = entry.pontuacao.ToString();
            //Prepara o handler de click do botão da lixeira que aparece ao lado de cada player na UI
            temp.AddDeleteHandler(OnDeleteHanler);
            //Armazenando ID
            playersId.Add(entry.id);
        }
        temp = null;
    }

    /// <summary>
    /// Este método limpa a lista de ranking na UI, destroi todos os objetos da lista
    /// </summary>
    private void ClearContainer()
    {
        for(int i = 0; i < rankingContainer.transform.childCount; i++)
        {
            Destroy(rankingContainer.transform.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// <remarks>
    /// <para>Este método limpa todos os campos do formulário de novo jogador</para>
    /// <para>Este método é chamado sempre ao final do método <see cref="OnSavePlayer"/></para>
    /// </remarks>
    /// </summary>
    private void ClearInputs()
    {
        inputId.text = string.Empty;
        inputNome.text = string.Empty;
        inputPonto.text = string.Empty;
    }
    #endregion Private Methods

    #region UI Action
    /// <summary>
    /// <remarks>
    /// <para>Este método remove um player do Ranking</para>
    /// <para>Primeiro ele remove o ID da lista de playersId e em seguida chama o serviço de apagar o player do ranking</para>
    /// </remarks>
    /// </summary>
    /// <param name="playerId">id do jogador escolhido</param>
    private void OnDeleteHanler(int playerId)
    {
        //Removendo o player da lista de ids
        playersId.Remove(playerId);
        //Chamando o serviço 
        service.DeletePlayer(playerId);
    }
    /// <summary>
    /// <remarks>
    /// <para>Este método é chamado sempre que o botão New Player é clicado</para>
    /// <para>É criado um objeto <see cref="Player"/> utilizando os dados do formulário de novo jogador</para>
    /// <para>em seguida é chamado o serviço <see cref="RankingService.PostNewPlayer(Player)"/></para>
    /// <para>Independente do retorno do serviço o método <see cref="ClearInputs"/> é chamado para limpar o formulário</para>
    /// </remarks>
    /// </summary>
    public void OnSavePlayer()
    {
        (bool erro, Player player) = PreparePlayerData();

        if (!erro)
        {
            if (ValidatePlayerId(player))
            {
                service.PostNewPlayer(player);
            }
        }
        ClearInputs();
    }
    /// <summary>
    /// Este método valida se o id informado no formulario já existe na lista de ranking
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private bool ValidatePlayerId(Player player)
    {
        if (playersId.Contains(player.id))
        {
            Debug.LogError("Este ID já existe no ranking!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Verifica se os dados do formulário estão preenchidos corretamente e devolve um objeto <see cref="Player"/>
    /// </summary>
    /// <returns></returns>
    private (bool, Player) PreparePlayerData()
    {
        bool erro = false;
        var player = new Player();

        if (inputId.text != string.Empty && int.TryParse(inputId.text, out int playerId))
        {
            player.id = playerId;
        }
        else
        {
            Debug.LogError("Por favor informe o ID do jogaor");
            erro = true;
        }

        if (inputNome.text != string.Empty)
        {
            player.nome = inputNome.text;
        }
        else
        {
            Debug.LogError("Por favor informe o nome do jogaor");
            erro = true;
        }

        if (!string.IsNullOrEmpty(inputPonto.text) && int.TryParse(inputPonto.text, out int playerScore))
        {
            player.pontuacao = playerScore;
        }
        else
        {
            Debug.LogError("Por favor informe a pontuação do jogaor");
            erro = true;
        }
        
        return (erro, player);
    }
    #endregion UI Action
}
