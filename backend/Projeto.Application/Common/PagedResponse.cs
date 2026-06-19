namespace Projeto.Application.Common;

// Classe genérica, o <T> é um coringa que aceita qualquer tipo
// PagedResponse<Produto>, PagedResponse<Cliente>, PagedResponse<Venda>
public class PagedResponse<T>
{
    public List<T> Data { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
