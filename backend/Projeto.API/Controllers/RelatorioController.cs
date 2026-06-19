using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly IRelatorioApplication _relatorioApplication;

    public RelatorioController(IRelatorioApplication relatorioApplication)
    {
        _relatorioApplication = relatorioApplication;
    }

    // Retorna os dados da VIEW vw_ResumoVendas
    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo()
    {
        var resultado = await _relatorioApplication.BuscarResumoAsync();
        return Ok(resultado);
    }

    // Retorna os dados da VIEW vw_ProdutosAbaixoMinimo
    [HttpGet("produtos-abaixo-minimo")]
    public async Task<IActionResult> ProdutosAbaixoMinimo()
    {
        var resultado = await _relatorioApplication.ListarProdutosAbaixoMinimoAsync();
        return Ok(resultado);
    }

    // Retorna os dados da VIEW vw_VendasPorFormaPagamento
    [HttpGet("formas-pagamento")]
    public async Task<IActionResult> FormasPagamento()
    {
        var resultado = await _relatorioApplication.ListarVendasPorFormaPagamentoAsync();
        return Ok(resultado);
    }

    // Executa a SP sp_ProdutosMaisVendidos com o parâmetro top
    [HttpGet("produtos-mais-vendidos")]
    public async Task<IActionResult> ProdutosMaisVendidos([FromQuery] int top = 10)
    {
        var resultado = await _relatorioApplication.ListarProdutosMaisVendidosAsync(top);
        return Ok(resultado);
    }

    // Executa a FUNCTION fn_TotalVendasCliente(@ClienteID)
    // Retorna: { "total": 1234.56 }
    [HttpGet("total-cliente/{clienteId}")]
    public async Task<IActionResult> TotalCliente(int clienteId)
    {
        var total = await _relatorioApplication.ConsultarTotalClienteAsync(clienteId);
        return Ok(new { total });
    }
}
