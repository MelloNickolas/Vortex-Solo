using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly IAIApplication _aiApplication;

    public AIController(IAIApplication aiApplication)
    {
        _aiApplication = aiApplication;
    }

    // POST /api/ai/analisar-vendas — VIEW vw_ResumoVendas
    [HttpPost("analisar-vendas")]
    public async Task<IActionResult> AnalisarVendas()
    {
        var (dados, resposta) = await _aiApplication.AnalisarVendasAsync();
        return Ok(new { dados, resposta });
    }

    // POST /api/ai/analisar-estoque — VIEW vw_ProdutosAbaixoMinimo
    [HttpPost("analisar-estoque")]
    public async Task<IActionResult> AnalisarEstoque()
    {
        var (dados, resposta) = await _aiApplication.AnalisarEstoqueAsync();
        return Ok(new { dados, resposta });
    }

    // POST /api/ai/analisar-formas-pagamento — VIEW vw_VendasPorFormaPagamento
    [HttpPost("analisar-formas-pagamento")]
    public async Task<IActionResult> AnalisarFormasPagamento()
    {
        var (dados, resposta) = await _aiApplication.AnalisarFormasPagamentoAsync();
        return Ok(new { dados, resposta });
    }

    // POST /api/ai/analisar-produtos-mais-vendidos — SP sp_ProdutosMaisVendidos
    [HttpPost("analisar-produtos-mais-vendidos")]
    public async Task<IActionResult> AnalisarProdutosMaisVendidos()
    {
        var (dados, resposta) = await _aiApplication.AnalisarProdutosMaisVendidosAsync();
        return Ok(new { dados, resposta });
    }

    // POST /api/ai/analisar-cliente/{clienteId} — FUNCTION fn_TotalVendasCliente
    [HttpPost("analisar-cliente/{clienteId}")]
    public async Task<IActionResult> AnalisarCliente(int clienteId)
    {
        var (dados, resposta) = await _aiApplication.AnalisarClienteAsync(clienteId);
        return Ok(new { dados, resposta });
    }
}
