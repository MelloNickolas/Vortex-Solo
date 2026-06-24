using Projeto.Repository.FuncoesSQL;

namespace Projeto.Repository.Interfaces;

public interface IDashboardRepository
{
    // Retorna o faturamento por dia de um mês/ano específico
    Task<List<VendaPorDiaResponse>> ListarVendasPorDiaAsync(int mes, int ano);
}
