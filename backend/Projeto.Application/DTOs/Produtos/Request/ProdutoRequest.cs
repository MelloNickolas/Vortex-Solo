namespace Projeto.Application.DTOs.Produtos.Request;

public class ProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int EstoqueAtual { get; set; }
    public int EstoqueMinimo { get; set; }
    public int CategoriaID { get; set; }
}
