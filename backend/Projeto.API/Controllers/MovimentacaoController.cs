using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.DTOs.Movimentacoes.Request;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MovimentacaoController : ControllerBase
{
    private readonly IMovimentacaoApplication _movimentacaoApplication;

    public MovimentacaoController(IMovimentacaoApplication movimentacaoApplication)
    {
        _movimentacaoApplication = movimentacaoApplication;
    }

    // GET /api/movimentacao?page=1&pageSize=10&produtoId=5&tipo=Entrada&de=2026-01-01&ate=2026-12-31
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? produtoId = null,
        [FromQuery] string? tipo = null,
        [FromQuery] DateTime? de = null,
        [FromQuery] DateTime? ate = null)
    {
        var response = await _movimentacaoApplication.ListarPagedAsync(page, pageSize, produtoId, tipo, de, ate);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] MovimentacaoRequest request)
    {
        var response = await _movimentacaoApplication.CriarAsync(request);
        return Created(string.Empty, response);
    }
}
