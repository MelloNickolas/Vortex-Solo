using Projeto.Application.Interfaces;
using Projeto.Repository.FuncoesSQL;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class RelatorioApplication : IRelatorioApplication
{
    private readonly IRelatorioRepository _relatorioRepository;

    public RelatorioApplication(IRelatorioRepository relatorioRepository)
    {
        _relatorioRepository = relatorioRepository;
    }

    public Task<ResumoVendasResponse?> BuscarResumoAsync()
    {
        return _relatorioRepository.BuscarResumoAsync();
    }

    public Task<List<ProdutoAbaixoMinimoResponse>> ListarProdutosAbaixoMinimoAsync()
    {
        return _relatorioRepository.ListarProdutosAbaixoMinimoAsync();
    }

    public Task<List<VendaPorFormaPagamentoResponse>> ListarVendasPorFormaPagamentoAsync()
    {
        return _relatorioRepository.ListarVendasPorFormaPagamentoAsync();
    }

    public Task<List<ProdutoMaisVendidoResponse>> ListarProdutosMaisVendidosAsync(int top = 10)
    {
        return _relatorioRepository.ListarProdutosMaisVendidosAsync(top);
    }

    public Task<decimal> ConsultarTotalClienteAsync(int clienteId)
    {
        return _relatorioRepository.ConsultarTotalClienteAsync(clienteId);
    }
}
