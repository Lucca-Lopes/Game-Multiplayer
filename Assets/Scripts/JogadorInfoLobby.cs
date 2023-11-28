using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class JogadorInfoLobby : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nome;
    [SerializeField] Button prontoBtn;
    //[SerializeField] Button sairBtn;
    [SerializeField] Image prontoIcon;

    string id;
    public string Id { get { return id; } }

    public string Nome
    {
        get
        {
            return nome.text;
        }
        set
        {
            nome.text = value;
        }
    }

    public void IniciarJogador(Player player, bool isLocal)
    {
        id = player.Id;
        AtualizarNome(player);
        //AtualizarPlayerLocal(player, isLocal);
        AtualizarBotaoPronto(player, isLocal);
    }

    void AtualizarNome(Player player)
    {
        if (player.Data.TryGetValue("nome", out PlayerDataObject obj))
        {
            Nome = obj.Value;
        }
    }

    void AtualizarBotaoPronto(Player player, bool isLocal)
    {
        bool valor = false;
        if (player.Data.TryGetValue("pronto", out PlayerDataObject obj))
        {
            valor = bool.Parse(obj.Value);
        }
        if (isLocal)
            prontoBtn.gameObject.SetActive(!valor);
        else
            prontoBtn.gameObject.SetActive(false);
        prontoIcon.gameObject.SetActive(valor);
    }

    //void AtualizarPlayerLocal(Player player, bool isLocal)
    //{
    //    if (isLocal)
    //    {
    //        sairBtn.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        sairBtn.gameObject.SetActive(false);
    //    }
    //}

    public void ProntoAction()
    {
        MenuGameManager.Lobby.AtualizarJogadorPronto(Id, true);
        //Chamar a atualização de status do jogador pronto no lobby
        prontoBtn.gameObject.SetActive(false);
        prontoIcon.gameObject.SetActive(true);
    }
    public void SairAction()
    {
        MenuGameManager.Lobby.SairLobby(MenuGameManager.PlayerId);
    }
}
