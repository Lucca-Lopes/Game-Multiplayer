using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// Esta classe representa os dados de tela de cada player na lista de ranking
/// </summary>
public class RankingEntryUI : MonoBehaviour
{
    [Header("Ranking")]
    [SerializeField] int id;
    [SerializeField] string nome;
    [SerializeField] string pontuacao;
    
    [Space]
    [Header("UI Components")]
    [SerializeField] TextMeshProUGUI idLabel;
    [SerializeField] TextMeshProUGUI nomeLabel;
    [SerializeField] TextMeshProUGUI pontuacaoLabel;
    [SerializeField] Button deleteButton;

    UnityAction<int> deleteAction;

    #region Unity Update
    private void Start()
    {
        deleteButton.onClick.AddListener(OnDeleteEntry);
    }
    #endregion Unity Update

    #region Properties
    /// <summary>
    /// Propriedades de acesso ao dado de tela do ranking
    /// </summary>
    public int ID 
    { 
        get 
        { 
            return id; 
        } 
        set 
        { 
            id = value;
            idLabel.text = id.ToString();
        } 
    }
    public string Nome 
    { 
        get 
        { 
            return nome; 
        } 
        set 
        { 
            nome = value;
            nomeLabel.text = nome;
        } 
    }
    public string Pontos 
    { 
        get 
        { 
            return pontuacao; 
        } 
        set 
        { 
            pontuacao = value;
            pontuacaoLabel.text = pontuacao;
        } 
    }
    #endregion Properties

    #region Button Action
    public void OnDeleteEntry()
    {
        deleteAction?.Invoke(id);
    }
    public void AddDeleteHandler(UnityAction<int> action)
    {
        deleteAction = action;
    }
    #endregion Button Action
}
