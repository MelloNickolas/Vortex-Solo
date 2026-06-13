namespace Projeto.Application.DTOs.Produtos.Response;

public class ProdutoResponse
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int EstoqueAtual { get; set; }
    public int EstoqueMinimo { get; set; }

    public int CategoriaID { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
}
