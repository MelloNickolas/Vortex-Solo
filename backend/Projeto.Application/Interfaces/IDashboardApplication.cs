using Projeto.Repository.FuncoesSQL;

namespace Projeto.Application.Interfaces;

public interface IDashboardApplication
{
    Task<List<VendaPorDiaResponse>> ListarVendasPorDiaAsync(int mes, int ano);
}
