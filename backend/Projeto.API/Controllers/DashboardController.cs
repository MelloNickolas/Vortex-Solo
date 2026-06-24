using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardApplication _dashboardApplication;

    public DashboardController(IDashboardApplication dashboardApplication)
    {
        _dashboardApplication = dashboardApplication;
    }

    // GET /api/dashboard/vendas-por-dia?mes=6&ano=2026
    // Retorna o faturamento por dia do mês/ano informado
    [HttpGet("vendas-por-dia")]
    public async Task<IActionResult> VendasPorDia([FromQuery] int mes, [FromQuery] int ano)
    {
        var resultado = await _dashboardApplication.ListarVendasPorDiaAsync(mes, ano);
        return Ok(resultado);
    }
}
