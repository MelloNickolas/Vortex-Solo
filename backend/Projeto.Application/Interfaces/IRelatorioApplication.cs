using Projeto.Domain.DTOs;

namespace Projeto.Application.Interfaces;

public interface IRelatorioApplication
{
    Task<ResumoVendasResponse?> BuscarResumoAsync();
    Task<List<ProdutoAbaixoMinimoResponse>> ListarProdutosAbaixoMinimoAsync();
    Task<List<VendaPorFormaPagamentoResponse>> ListarVendasPorFormaPagamentoAsync();
    Task<List<ProdutoMaisVendidoResponse>> ListarProdutosMaisVendidosAsync(int top = 10);
}
