namespace Projeto.Application.DTOs.Movimentacoes.Response;

public class MovimentacaoResponse
{
    public int ID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public DateTime DataMovimento { get; set; }

    public int ProdutoID { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;

    public int UsuarioID { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
}
