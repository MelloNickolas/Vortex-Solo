using Projeto.Application.Interfaces;
using Projeto.Repository.FuncoesSQL;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class DashboardApplication : IDashboardApplication
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardApplication(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public Task<List<VendaPorDiaResponse>> ListarVendasPorDiaAsync(int mes, int ano)
    {
        return _dashboardRepository.ListarVendasPorDiaAsync(mes, ano);
    }
}
