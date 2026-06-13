namespace Projeto.Application.DTOs.Clientes.Response;

public class ClienteResponse
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public bool Ativo { get; set; }

    // Retorna ID e Nome da cidade — frontend não precisa fazer segunda chamada
    public int CidadeID { get; set; }
    public string CidadeNome { get; set; } = string.Empty;
}
