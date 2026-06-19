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

    [HttpPost("analisar-vendas")]
    public async Task<IActionResult> AnalisarVendas()
    {
        var resposta = await _aiApplication.AnalisarVendasAsync();
        return Ok(new { resposta }); // isso pq o front acessa com data.resposta, e fica mais organizado
    }

    [HttpPost("analisar-estoque")]
    public async Task<IActionResult> AnalisarEstoque()
    {
        var resposta = await _aiApplication.AnalisarEstoqueAsync();
        return Ok(new { resposta });
    }
}
