namespace Projeto.Repository.FuncoesSQL;

// VIEW vw_ProdutosAbaixoMinimo
// Lista os produtos com EstoqueAtual abaixo do EstoqueMinimo definido no cadastro.
public class ProdutoAbaixoMinimoResponse
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int EstoqueAtual { get; set; }
    public int EstoqueMinimo { get; set; }
    public int Deficit { get; set; }
}
