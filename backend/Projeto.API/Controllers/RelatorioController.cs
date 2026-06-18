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

    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo()
        => Ok(await _relatorioApplication.BuscarResumoAsync());

    [HttpGet("produtos-abaixo-minimo")]
    public async Task<IActionResult> ProdutosAbaixoMinimo()
        => Ok(await _relatorioApplication.ListarProdutosAbaixoMinimoAsync());

    [HttpGet("formas-pagamento")]
    public async Task<IActionResult> FormasPagamento()
        => Ok(await _relatorioApplication.ListarVendasPorFormaPagamentoAsync());

    [HttpGet("produtos-mais-vendidos")]
    public async Task<IActionResult> ProdutosMaisVendidos([FromQuery] int top = 10)
        => Ok(await _relatorioApplication.ListarProdutosMaisVendidosAsync(top));
}
