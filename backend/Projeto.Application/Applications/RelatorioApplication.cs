using Projeto.Application.Interfaces;
using Projeto.Domain.DTOs;
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
        => _relatorioRepository.BuscarResumoAsync();

    public Task<List<ProdutoAbaixoMinimoResponse>> ListarProdutosAbaixoMinimoAsync()
        => _relatorioRepository.ListarProdutosAbaixoMinimoAsync();

    public Task<List<VendaPorFormaPagamentoResponse>> ListarVendasPorFormaPagamentoAsync()
        => _relatorioRepository.ListarVendasPorFormaPagamentoAsync();

    public Task<List<ProdutoMaisVendidoResponse>> ListarProdutosMaisVendidosAsync(int top = 10)
        => _relatorioRepository.ListarProdutosMaisVendidosAsync(top);
}
