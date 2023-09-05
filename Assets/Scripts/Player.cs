
/// <summary>
/// <remarks>
/// <para>Esta classe representa os dados do Player, todos os players existentes no ranking possuem uma representação dessa classe</para>
/// </remarks>
/// </summary>
[System.Serializable]
public class Player
{
    public int id;
    public string nome;
    public int pontuacao;

    public Player(int id, string nome, int pontuacao)
    {
        this.id = id;
        this.nome = nome;
        this.pontuacao = pontuacao;
    }

    public Player() { }

}
