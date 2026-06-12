namespace Projeto.Application.Common;

// Classe genérica, o <T> é um coringa que aceita qualquer tipo
// Exemplos: PagedResponse<Produto>, PagedResponse<Cliente>, PagedResponse<Venda>
// Isso evita criar uma classe de resposta paginada para cada módulo
public class PagedResponse<T>
{
    // Os registros da página atual
    // Exemplo: página 2 com 10 itens → aqui vêm esses 10 itens
    public List<T> Data { get; set; } = [];

    // Total de registros no banco COM os filtros aplicados
    // Exemplo: busquei "Note" e achei 23 produtos → Total = 23
    // O frontend usa esse número para calcular quantas páginas existem: 23 ÷ 10 = 3 páginas
    public int Total { get; set; }

    // Página que o usuário está visualizando no momento
    // Exemplo: usuário clicou na página 2 → Page = 2
    public int Page { get; set; }

    // Quantidade de registros que cabem em cada página
    // Exemplo: configurado para 10 itens por página → PageSize = 10
    public int PageSize { get; set; }
}
