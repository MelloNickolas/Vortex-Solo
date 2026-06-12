namespace Projeto.Domain.Entities;

public class Produto
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int EstoqueAtual { get; set; }
    public int EstoqueMinimo { get; set; } // vamos usar para gerar alertas e avisos

    public int CategoriaID { get; set; }
    public Categoria Categoria { get; set; } = null!;


    public ICollection<ItemVenda> ItensVenda { get; set; } = [];
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = [];
}
