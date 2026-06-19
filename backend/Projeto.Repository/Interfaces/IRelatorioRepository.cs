using Projeto.Repository.FuncoesSQL;

namespace Projeto.Repository.Interfaces;

public interface IRelatorioRepository
{
    Task<ResumoVendasResponse?> BuscarResumoAsync();
    Task<List<ProdutoAbaixoMinimoResponse>> ListarProdutosAbaixoMinimoAsync();
    Task<List<VendaPorFormaPagamentoResponse>> ListarVendasPorFormaPagamentoAsync();
    Task<List<ProdutoMaisVendidoResponse>> ListarProdutosMaisVendidosAsync(int top);

    // FUNCTION: fn_TotalVendasCliente
    Task<decimal> ConsultarTotalClienteAsync(int clienteId);
}


